using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using CoMon.Configuration;
using CoMon.Statuses;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Retention
{
    public class RetentionWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IRepository<Status, long> _statusRepository;
        private readonly ILogger<RetentionWorker> _logger;

        private const int WorkerCycleSeconds = 5 * 60;


        public RetentionWorker(AbpAsyncTimer timer, IRepository<Status, long> statusRepository,
            ILogger<RetentionWorker> logger) : base(timer)
        {
            Timer.Period = WorkerCycleSeconds * 1000;
            _statusRepository = statusRepository;
            _logger = logger;
        }

        [UnitOfWork]
        protected override async Task DoWorkAsync()
        {
            try
            {
                _logger.LogInformation("Starting retention worker job.");

                var retentionDays = await SettingManager.GetSettingValueAsync<int>(AppSettingNames.RetentionDays);

                if (retentionDays < 0)
                {
                    _logger.LogInformation("Aborting retention worker job because retention days are negative.");
                    return;
                }

                var cutOffDate = DateTime.UtcNow.AddDays(-retentionDays);

                var itemsToDelete = _statusRepository
                    .GetAll()
                    .Where(s => s.Criticality != null)
                    .Where(s => s.Time < cutOffDate);

                _statusRepository.RemoveRange(itemsToDelete);

                _logger.LogInformation("Successfully finished retention worker job.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during retention worker job: {message}", ex.Message);
            }
        }
    }
}
