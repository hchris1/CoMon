using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
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

        public ExternalAppService(IRepository<Package, long> packageRepository,
            IRepository<Status, long> statusRepository, IObjectMapper mapper)
        {
            _packageRepository = packageRepository;
            _statusRepository = statusRepository;
            _mapper = mapper;
        }

        public async Task<long> CreateStatus(Guid packageGuid, CreateStatusDto input)
        {
            var package = _packageRepository
                .GetAll()
                .Include(p => p.Asset)
                .Where(p => p.Guid == packageGuid)
                .SingleOrDefault()
                ?? throw new EntityNotFoundException($"Package not found for GUID {packageGuid}.");

            var status = _mapper.Map<Status>(input);
            status.Time = DateTime.Now;
            status.Package = package;
            return await _statusRepository.InsertAndGetIdAsync(status);
        }
    }
}
