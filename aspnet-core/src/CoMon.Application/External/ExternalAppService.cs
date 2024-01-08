using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.External.Dtos;
using CoMon.Packages;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.External
{
    public class ExternalAppService(IRepository<Package, long> packageRepository,
        IRepository<Status, long> statusRepository, IObjectMapper mapper) : CoMonAppServiceBase
    {
        private readonly IRepository<Package, long> _packageRepository = packageRepository;
        private readonly IRepository<Status, long> _statusRepository = statusRepository;
        private readonly IObjectMapper _mapper = mapper;

        public async Task<long> CreateStatus(Guid packageGuid, CreateStatusDto input)
        {
            var package = _packageRepository
                .GetAll()
                .Include(p => p.Asset)
                .Where(p => p.Guid == packageGuid)
                .AsSplitQuery()
                .SingleOrDefault()
                ?? throw new EntityNotFoundException($"Package not found for GUID {packageGuid}.");

            var status = _mapper.Map<Status>(input);
            status.Time = DateTime.UtcNow;
            status.Package = package;

            return await _statusRepository.InsertAndGetIdAsync(status);
        }
    }
}
