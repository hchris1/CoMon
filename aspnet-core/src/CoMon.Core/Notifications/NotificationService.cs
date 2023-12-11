using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using CoMon.Packages;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Notifications
{
    public class NotificationService(CoMonHub comonHub, IUnitOfWorkManager unitOfWorkManager, IRepository<Package, long> packageRepository, ILogger<NotificationService> logger)
        : ISingletonDependency, INotificationService, IAsyncEventHandler<EntityCreatedEventData<Status>>
    {
        private readonly ILogger<NotificationService> _logger = logger;
        private readonly CoMonHub _comonHub = comonHub;
        private readonly IRepository<Package, long> _packageRepository = packageRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager = unitOfWorkManager;

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
                        .ThenInclude(a => a.Group.Parent.Parent)
                        .Include(p => p.Statuses
                            .Where(s => s.Time < status.Time)
                            .OrderByDescending(s => s.Time)
                            .Take(1))
                        .FirstOrDefaultAsync();
                uow.Complete();

                // Gather group ids
                var groupIds = new List<long>();
                var group = package.Asset.Group;
                while (group != null)
                {
                    groupIds.Add(group.Id);
                    group = group.Parent;
                }

                var update = new StatusUpdateDto()
                {
                    Id = status.Id,
                    Time = status.Time,
                    PreviousCriticality = package.LastStatus?.Criticality,
                    Criticality = status.Criticality,
                    PackageId = package.Id,
                    PackageName = package.Name,
                    AssetId = package.Asset.Id,
                    AssetName = package.Asset.Name,
                    GroupIds = groupIds
                };

                await _comonHub.SendStatusUpdate(update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send status update");
            }
        }
    }
}
