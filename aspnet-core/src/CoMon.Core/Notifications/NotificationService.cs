using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Notifications
{
    public class NotificationService : ISingletonDependency
    {
        private readonly CoMonHub _comonHub;
        private readonly IRepository<Status, long> _statusRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public NotificationService(CoMonHub comonHub, IRepository<Status, long> statusRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _comonHub = comonHub;
            _statusRepository = statusRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task SendStatusUpdate(Status status)
        {
            await _comonHub.SendStatusUpdate(status);
            await CheckAndSendStatusChange(status);
        }

        private async Task CheckAndSendStatusChange(Status status)
        {
            using var uow = _unitOfWorkManager.Begin();
            var previousCriticality = _statusRepository
                .GetAll()
                .Include(s => s.Package)
                .Where(s => s.Package.Id == status.Package.Id)
                .Where(s => s.Time < status.Time)
                .OrderByDescending(s => s.Time)
                .Select(s => s.Criticality)
                .FirstOrDefault();
            uow.Complete();

            if (previousCriticality == status.Criticality)
                return;

            await _comonHub.SendStatusChange(status, previousCriticality);
        }
    }
}
