using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Packages
{
    public abstract class PackageWorkerBase<TSettings> : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
        where TSettings : class
    {
        private readonly IRepository<Package, long> packageRepository;
        protected readonly IRepository<Status, long> statusRepository;
        protected readonly ILogger logger;
        private readonly ConcurrentDictionary<long, bool> manualCheckDict = new();

        protected const int WorkerCycleSeconds = 5;

        protected PackageWorkerBase(AbpAsyncTimer timer, IRepository<Package, long> packageRepo, ILogger log, IRepository<Status, long> statusRepo)
            : base(timer)
        {
            packageRepository = packageRepo;
            logger = log;
            statusRepository = statusRepo;
            Timer.Period = WorkerCycleSeconds * 1000;
        }

        public void EnqueueManualCheck(long packageId)
        {
            logger.LogError("Enqueue package with {packageId}", packageId);
            manualCheckDict.TryAdd(packageId, true);
        }

        [UnitOfWork]
        protected override async Task DoWorkAsync()
        {
            var packages = await LoadPackages();
            foreach (var package in packages)
            {
                try
                {
                    var isManuallyQueued = manualCheckDict.TryRemove(package.Id, out _);
                    if (!ShouldPerformCheck(package) && !isManuallyQueued)
                        continue;

                    var status = await PerformCheck(package);
                    status.PackageId = package.Id;
                    await statusRepository.InsertAsync(status);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error while performing check for package with id {package.Id}: {ex.Message}");
                }
            }
        }

        protected virtual Task<List<Package>> LoadPackages()
        {
            return packageRepository
                .GetAll()
                .Include(p => p.Asset)
                .Include(p => p.PingPackageSettings)
                .Include(p => p.HttpPackageSettings)
                .Include(p => p.RtspPackageSettings)
                .Include(p => p.Statuses
                    .Where(s => s.Criticality != null)
                    .OrderByDescending(s => s.Time).Take(1))
                .Where(p => p.Type == Type)
                .AsSplitQuery()
                .ToListAsync();
        }

        protected abstract Task<Status> PerformCheck(Package package);

        protected bool ShouldPerformCheck(Package package)
        {
            var lastStatus = package.Statuses.FirstOrDefault();
            if (lastStatus == null)
                return true;

            var settings = package.GetSettings<TSettings>();
            if (settings == null)
            {
                logger.LogWarning("Skipping check for package with id {packageId} because its settings are null.", package.Id);
                return false;
            }

            var timeSinceLastRun = DateTime.UtcNow - lastStatus.Time;
            return timeSinceLastRun.TotalSeconds > settings.CycleSeconds;
        }

        protected abstract PackageType Type { get; }
    }
}
