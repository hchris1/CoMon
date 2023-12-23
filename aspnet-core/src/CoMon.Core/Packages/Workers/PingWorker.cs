using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace CoMon.Packages.Workers
{
    public class PingWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IRepository<Package, long> _packageRepository;
        private readonly IRepository<Status, long> _statusRepository;
        private readonly ILogger<PingWorker> _logger;

        private const int WorkerCycleSeconds = 5;

        public PingWorker(AbpAsyncTimer timer, IRepository<Package, long> packageRepository,
            ILogger<PingWorker> logger, IRepository<Status, long> statusRepository) : base(timer)
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
                    .Include(p => p.PingPackageSettings)
                    .Include(p => p.Asset)
                    .Include(p => p.Statuses
                        .Where(s => s.Criticality != null)
                        .OrderByDescending(s => s.Time)
                        .Take(1))
                    .Where(p => p.Type == PackageType.Ping)
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
                    _logger.LogError("Error while performing ping check for package with id {packageId}: {message}", package.Id, ex.Message);
                }
            }
        }

        private async Task<Status> PerformCheck(Package package)
        {
            try
            {
                var pingSender = new Ping();
                var reply = await pingSender.SendPingAsync(package.PingPackageSettings.Host);
                return CreateStatus(package.PingPackageSettings.Host, reply);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while performing ping check for package with id {packageId}: {message}", package.Id, ex.Message);
                return new Status
                {
                    Time = DateTime.UtcNow,
                    Criticality = Criticality.Alert,
                    Messages = ["An error occurred when running the ping check. Check the log for details."]
                };
            }
        }

        private bool ShouldPerformCheck(Package package)
        {
            var lastStatus = package.Statuses.FirstOrDefault();
            if (lastStatus == null)
                return true;

            if (package.PingPackageSettings == null)
            {
                _logger.LogWarning("Skipping ping check for package with id {packageId} because the ping settings are null.", package.Id);
                return false;
            }

            var timeSinceLastRun = DateTime.UtcNow - lastStatus.Time;

            return timeSinceLastRun.TotalSeconds > package.PingPackageSettings.CycleSeconds;
        }

        private static Status CreateStatus(string host, PingReply reply)
        {
            var pingable = reply.Status == IPStatus.Success;

            return new Status
            {
                Time = DateTime.UtcNow,
                Criticality = pingable ? Criticality.Healthy : Criticality.Alert,
                Messages = pingable ? [$"Successfully pinged {host}."] : [$"Unable to ping {host}."],
                KPIs = pingable ?
                    [
                        new KPI()
                        {
                            Name = "RoundTrip time",
                            Unit = "ms",
                            Value = reply.RoundtripTime
                        }
                    ]
                    : []
            };
        }
    }
}
