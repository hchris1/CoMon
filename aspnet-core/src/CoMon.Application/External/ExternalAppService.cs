using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Notifications;
using CoMon.Packages;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.External
{
    public class ExternalAppService : CoMonAppServiceBase
    {
        private readonly IRepository<Package, long> _packageRepository;
        private readonly IRepository<Status, long> _statusRepository;
        private readonly IObjectMapper _mapper;
        private readonly NotificationService _notificationService;

        public ExternalAppService(IRepository<Package, long> packageRepository,
            IRepository<Status, long> statusRepository, IObjectMapper mapper, NotificationService notificationService)
        {
            _packageRepository = packageRepository;
            _statusRepository = statusRepository;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<long> CreateStatus(Guid packageGuid, CreateStatusDto input)
        {
            var package = _packageRepository
                .GetAll()
                .Include(p => p.Asset)
                .Where(p => p.Guid == packageGuid)
                .SingleOrDefault()
                ?? throw new EntityNotFoundException("Unable to identify package.");

            var status = _mapper.Map<Status>(input);
            status.Time = DateTime.Now;
            status.Package = package;
            var id = await _statusRepository.InsertAndGetIdAsync(status);

            await _notificationService.SendStatusUpdate(status);
            return id;
        }
    }
}
