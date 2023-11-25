using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using CoMon.Packages;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Notifications
{
    public class NotificationService : ISingletonDependency, INotificationService, IAsyncEventHandler<EntityCreatedEventData<Status>>
    {
        private readonly CoMonHub _comonHub;
        private readonly IRepository<Package, long> _packageRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public NotificationService(CoMonHub comonHub, IUnitOfWorkManager unitOfWorkManager, IRepository<Package, long> packageRepository)
        {
            _comonHub = comonHub;
            _unitOfWorkManager = unitOfWorkManager;
            _packageRepository = packageRepository;
        }

        public async Task HandleEventAsync(EntityCreatedEventData<Status> eventData)
        {
            try
            {
                using var uow = _unitOfWorkManager.Begin();
                var status = eventData.Entity;

                var package = await _packageRepository
                        .GetAll()
                        .Where(p => p.Id == status.PackageId)
                        .Include(p => p.Asset)
                        .Include(p => p.Statuses
                            .Where(s => s.Time < status.Time)
                            .OrderByDescending(s => s.Time)
                            .Take(1))
                        .FirstOrDefaultAsync();
                uow.Complete();

                status.Package = package;
                await _comonHub.SendStatusUpdate(status);

                if (status.Criticality != package.LastStatus?.Criticality)
                    await _comonHub.SendStatusChange(status, package.LastStatus.Criticality);

            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
        }
    }
}
