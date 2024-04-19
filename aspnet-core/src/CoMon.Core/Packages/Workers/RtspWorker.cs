﻿using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using CoMon.Common;
using CoMon.Packages.Settings;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rtsp.Messages;
using Rtsp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace CoMon.Packages.Workers
{
    public class RtspWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IRepository<Package, long> _packageRepository;
        private readonly IRepository<Status, long> _statusRepository;
        private readonly ILogger<RtspWorker> _logger;

        private const int WorkerCycleSeconds = 5;
        private const int TimeoutSeconds = 5;

        public RtspWorker(AbpAsyncTimer timer, IRepository<Package, long> packageRepository,
            ILogger<RtspWorker> logger, IRepository<Status, long> statusRepository) : base(timer)
        {
            Timer.Period = WorkerCycleSeconds * 1000;
            _packageRepository = packageRepository;
            _logger = logger;
            _statusRepository = statusRepository;
        }

        [UnitOfWork]
        protected override async Task DoWorkAsync()
        {
            var packages = await _packageRepository
                    .GetAll()
                    .Include(p => p.RtspPackageSettings)
                    .Include(p => p.Asset)
                    .Include(p => p.Statuses
                        .Where(s => s.Criticality != null)
                        .OrderByDescending(s => s.Time)
                        .Take(1))
                    .Where(p => p.Type == PackageType.Rtsp)
                    .AsSplitQuery()
                    .ToListAsync();

            foreach (var package in packages)
            {
                try
                {
                    if (!ShouldPerformCheck(package))
                        continue;

                    var status = await PerformCheck(package);
                    status.PackageId = package.Id;
                    await _statusRepository.InsertAsync(status);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error while performing rtsp check for package with id {packageId}: {message}", package.Id, ex.Message);
                }
            }
        }

        private async Task<Status> PerformCheck(Package package)
        {
            try
            {
                var url = new Uri(package.RtspPackageSettings.Url);
                var stopWatch = Stopwatch.StartNew();
                var isHealthy = await CheckHealth(url, package.RtspPackageSettings.Method);
                return CreateStatus(isHealthy, url.ToMaskedString(),
                    package.RtspPackageSettings.Method.ToString(), stopWatch.Elapsed);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while performing http check for package with id {packageId}: {message}", package.Id, ex.Message);
                return new Status
                {
                    Time = DateTime.UtcNow,
                    Criticality = Criticality.Alert,
                    Messages = ["An error occurred when running the http check. Check the log for details."]
                };
            }
        }

        private async Task<bool> CheckHealth(Uri url, RtspPackageMethod method)
        {
            try
            {
                var tcpSocket = new RtspTcpTransport(url);
                var rtspClient = new RtspListener(tcpSocket);
                rtspClient.Start();

                var message = CreateRequestMessage(method);
                message.RtspUri = url;

                var isHealthy = false;
                var cancellationToken = new CancellationTokenSource();

                rtspClient.MessageReceived += (sender, message) =>
                {
                    isHealthy = true;
                    cancellationToken.Cancel();
                };

                rtspClient.SendMessage(message);

                try
                {
                    await Task.Delay(TimeoutSeconds * 1000, cancellationToken.Token);
                }
                catch (TaskCanceledException)
                {
                    // Expected when response is received
                }
                finally
                {
                    rtspClient.Stop();
                    rtspClient.Dispose();
                }

                return isHealthy;
            }
            catch (Exception ex)
            {
                _logger.LogError("Health check failed: {0}", ex.Message);
                return false;
            }
        }

        private RtspRequest CreateRequestMessage(RtspPackageMethod method)
        {
            return method switch
            {
                RtspPackageMethod.Describe => new RtspRequestDescribe(),
                RtspPackageMethod.Options => new RtspRequestOptions(),
                RtspPackageMethod.Play => new RtspRequestPlay(),
                RtspPackageMethod.Pause => new RtspRequestPause(),
                RtspPackageMethod.Teardown => new RtspRequestTeardown(),
                RtspPackageMethod.Setup => new RtspRequestSetup(),
                _ => throw new Exception($"Unknown rtsp method {method}"),
            };
        }

        private Status CreateStatus(bool isHealthy, string url, string method, TimeSpan responseTime)
        {
            return new Status
            {
                Time = DateTime.UtcNow,
                Criticality = isHealthy ? Criticality.Healthy : Criticality.Alert,
                Messages = [isHealthy
                    ? $"Successfully requested {url} ({method})."
                    : $"Unable to request {url} ({method})."],
                KPIs = isHealthy
                    ? [
                        new KPI()
                        {
                            Name = "Response Time",
                            Unit = "ms",
                            Value = responseTime.Milliseconds
                        }
                    ]
                    : []
            };
        }

        private bool ShouldPerformCheck(Package package)
        {
            var lastStatus = package.Statuses.FirstOrDefault();
            if (lastStatus == null)
                return true;

            if (package.RtspPackageSettings == null)
            {
                _logger.LogWarning("Skipping rtsp check for package with id {packageId} because the rtsp settings are null.", package.Id);
                return false;
            }

            var timeSinceLastRun = DateTime.UtcNow - lastStatus.Time;

            return timeSinceLastRun.TotalSeconds > package.RtspPackageSettings.CycleSeconds;
        }
    }
}
