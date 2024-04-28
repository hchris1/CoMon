using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
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

        protected const int WorkerCycleSeconds = 2;
        protected const int MaxDegreeOfParallelism = 10;

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
            manualCheckDict.TryAdd(packageId, true);
        }

        [UnitOfWork]
        protected override async Task DoWorkAsync()
        {
            var packages = await LoadPackages();
            var packagesToProcess = packages.Where(ShouldPerformCheck).ToList();

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelism
            };

            var statuses = new ConcurrentBag<Status>();

            await Parallel.ForEachAsync(packagesToProcess, options, async (package, ct) =>
            {
                try
                {
                    var status = await PerformCheck(package);
                    status.PackageId = package.Id;
                    statuses.Add(status);
                }
                catch (Exception ex)
                {
                    logger.LogError("Error while performing check for package with id {packageId}: {message}", package.Id, ex.Message);
                }
            });

            await statusRepository.InsertRangeAsync(statuses);
        }

        protected async Task<List<Package>> LoadPackages()
        {
            return await packageRepository
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

        public abstract Task<Status> PerformCheck(Package package);

        public bool ShouldPerformCheck(Package package)
        {
            var isManuallyQueued = manualCheckDict.TryRemove(package.Id, out _);
            if (isManuallyQueued)
                return true;

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
