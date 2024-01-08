using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoMon.Packages.Workers
{
    public class HttpWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IRepository<Package, long> _packageRepository;
        private readonly IRepository<Status, long> _statusRepository;
        private readonly ILogger<HttpWorker> _logger;

        private const int WorkerCycleSeconds = 5;
        private const int TimeoutSeconds = 5;

        public HttpWorker(AbpAsyncTimer timer, IRepository<Package, long> packageRepository,
            ILogger<HttpWorker> logger, IRepository<Status, long> statusRepository) : base(timer)
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
                    .Include(p => p.HttpPackageSettings)
                    .Include(p => p.Asset)
                    .Include(p => p.Statuses
                        .Where(s => s.Criticality != null)
                        .OrderByDescending(s => s.Time)
                        .Take(1))
                    .Where(p => p.Type == PackageType.Http)
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
                    _logger.LogError("Error while performing http check for package with id {packageId}: {message}", package.Id, ex.Message);
                }
            }
        }

        private async Task<Status> PerformCheck(Package package)
        {
            try
            {
                var handler = package.HttpPackageSettings.IgnoreSslErrors
                    ? new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator }
                    : new HttpClientHandler();

                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);

                HttpMethod method = package.HttpPackageSettings.Method switch
                {
                    HttpPackageMethod.Get => HttpMethod.Get,
                    HttpPackageMethod.Post => HttpMethod.Post,
                    HttpPackageMethod.Put => HttpMethod.Put,
                    HttpPackageMethod.Delete => HttpMethod.Delete,
                    HttpPackageMethod.Patch => HttpMethod.Patch,
                    _ => throw new Exception($"Unknown http method {package.HttpPackageSettings.Method}"),
                };

                using var requestMessage = new HttpRequestMessage(method, package.HttpPackageSettings.Url);

                // Add headers
                if (!string.IsNullOrWhiteSpace(package.HttpPackageSettings.Headers))
                    foreach (var header in JsonConvert.DeserializeObject<Dictionary<string, string>>(package.HttpPackageSettings.Headers))
                        requestMessage.Headers.Add(header.Key, header.Value);

                // Add body
                if (!string.IsNullOrWhiteSpace(package.HttpPackageSettings.Body))
                    requestMessage.Content = package.HttpPackageSettings.Encoding switch
                    {
                        HttpPackageBodyEncoding.Json => new StringContent(package.HttpPackageSettings.Body, Encoding.UTF8, "application/json"),
                        HttpPackageBodyEncoding.Xml => new StringContent(package.HttpPackageSettings.Body, Encoding.UTF8, "application/xml"),
                        _ => throw new Exception($"Unknown encoding {package.HttpPackageSettings.Encoding}")
                    };

                var stopWatch = Stopwatch.StartNew();
                var response = await client.SendAsync(requestMessage);
                return CreateStatus(response, package.HttpPackageSettings, stopWatch.Elapsed);
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

        private Status CreateStatus(HttpResponseMessage response, HttpPackageSettings settings, TimeSpan responseTime)
        {
            return new Status
            {
                Time = DateTime.UtcNow,
                Criticality = response.IsSuccessStatusCode ? Criticality.Healthy : Criticality.Alert,
                Messages = [response.IsSuccessStatusCode
                    ? $"Successfully requested {settings.Url} ({response.StatusCode})."
                    : $"Unable to request {settings.Url} ({response.StatusCode})."],
                KPIs = response.IsSuccessStatusCode
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

            if (package.HttpPackageSettings == null)
            {
                _logger.LogWarning("Skipping http check for package with id {packageId} because the http settings are null.", package.Id);
                return false;
            }

            var timeSinceLastRun = DateTime.UtcNow - lastStatus.Time;

            return timeSinceLastRun.TotalSeconds > package.HttpPackageSettings.CycleSeconds;
        }
    }
}
