
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Assets;
using CoMon.Assets.Dtos;
using CoMon.Groups;
using CoMon.Groups.Dtos;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Statuses
{
    public class StatusAppService(IRepository<Asset, long> assetRepository, IRepository<Group, long> groupRepository,
        IRepository<Status, long> statusRepository, IObjectMapper objectMapper) : CoMonAppServiceBase, IStatusAppService
    {
        private readonly IRepository<Asset, long> _assetRepository = assetRepository;
        private readonly IRepository<Group, long> _groupRepository = groupRepository;
        private readonly IRepository<Status, long> _statusRepository = statusRepository;
        private readonly IObjectMapper _objectMapper = objectMapper;

        public async Task<StatusDto> Get(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.Package)
                .ThenInclude(p => p.Asset)
                .ThenInclude(a => a.Group.Parent.Parent)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync() 
                ?? throw new EntityNotFoundException("Status not found.");

            status.IsLatest = await IsLatest(status);

            return _objectMapper.Map<StatusDto>(status);
        }

        private async Task<bool> IsLatest(Status status)
        {
            var latestId = await _statusRepository
                .GetAll()
                .Where(s => s.Package.Id == status.Package.Id)
                .OrderByDescending(s => s.Time)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
            return latestId == status.Id;
        }

        public async Task<StatusHistoryDto> GetHistory(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Include(s => s.Package)
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync() 
                ?? throw new EntityNotFoundException("Status not found.");

            var latestStatus = await _statusRepository
                .GetAll()
                .Where(s => s.Package.Id == status.Package.Id)
                .OrderByDescending(s => s.Time)
                .FirstOrDefaultAsync();

            var previousStatus = await _statusRepository
                .GetAll()
                .Where(s => s.Package.Id == status.Package.Id)
                .Where(s => s.Time < status.Time)
                .OrderByDescending(s => s.Time)
                .FirstOrDefaultAsync();

            var nextStatus = await _statusRepository
                .GetAll()
                .Where(s => s.Package.Id == status.Package.Id)
                .Where(s => s.Time > status.Time)
                .OrderBy(s => s.Time)
                .FirstOrDefaultAsync();

            if (status.Id == latestStatus.Id)
                latestStatus = null;


            return new StatusHistoryDto()
            {
                Status = _objectMapper.Map<StatusPreviewDto>(status),
                LatestStatus = _objectMapper.Map<StatusPreviewDto>(latestStatus),
                PreviousStatus = _objectMapper.Map<StatusPreviewDto>(previousStatus),
                NextStatus = _objectMapper.Map<StatusPreviewDto>(nextStatus)
            };
        }

        public async Task<StatusTableOptionsDto> GetStatusTableOptions()
        {
            var assets = await _assetRepository
                .GetAll()
                .Include(a => a.Group.Parent.Parent)
                .ToListAsync();

            var groups = await _groupRepository
                .GetAll()
                .Include(g => g.Parent.Parent)
                .ToListAsync();

            return new StatusTableOptionsDto()
            {
                Assets = _objectMapper.Map<List<AssetPreviewDto>>(assets),
                Groups = _objectMapper.Map<List<GroupPreviewDto>>(groups),
            };
        }

        public async Task<PagedResultDto<StatusPreviewDto>> GetStatusTable(PagedResultRequestDto request, long? assetId, long? groupId, long? packageId, Criticality? criticality, bool latestOnly = true)
        {
            IQueryable<Status> query = _statusRepository
                .GetAll()
                .OrderByDescending(s => s.Time)
                .Include(s => s.Package)
                .ThenInclude(p => p.Asset)
                .ThenInclude(a => a.Group.Parent.Parent);

            if (assetId != null)
                query = query.Where(s => s.Package.Asset.Id == assetId);

            if (groupId != null)
            {
                // WTH is this??
                query = query.Where(s =>
                    s.Package.Asset.Group.Id == groupId ||
                    s.Package.Asset.Group.Parent.Id == groupId ||
                    s.Package.Asset.Group.Parent.Parent.Id == groupId ||
                    s.Package.Asset.Group.Parent.Parent.Parent.Id == groupId ||
                    s.Package.Asset.Group.Parent.Parent.Parent.Parent.Id == groupId ||
                    s.Package.Asset.Group.Parent.Parent.Parent.Parent.Parent.Id == groupId ||
                    s.Package.Asset.Group.Parent.Parent.Parent.Parent.Parent.Parent.Id == groupId ||
                    s.Package.Asset.Group.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Id == groupId ||
                    s.Package.Asset.Group.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Id == groupId ||
                    s.Package.Asset.Group.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Id == groupId ||
                    s.Package.Asset.Group.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Id == groupId ||
                    s.Package.Asset.Group.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Id == groupId ||
                    s.Package.Asset.Group.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Id == groupId
                    );
            }

            if (packageId != null)
                query = query.Where(s => s.Package.Id == packageId);

            if (latestOnly)
            {
                var latestStatuses = await query.GroupBy(s => s.Package).Select(g => g.OrderByDescending(s => s.Time).FirstOrDefault()).ToListAsync();
                query = latestStatuses.AsQueryable();
            }

            if (criticality != null)
                query = query.Where(s => s.Criticality == criticality);

            var statuses = query
                .OrderByDescending(s => s.Time)
                .Skip(request.SkipCount)
                .Take(request.MaxResultCount)
                .ToList();

            var totalCount = query.Count();

            if (!latestOnly)
            {
                foreach (var status in statuses)
                    status.IsLatest = await IsLatest(status);
            }

            return new PagedResultDto<StatusPreviewDto>(
                totalCount,
                _objectMapper.Map<List<StatusPreviewDto>>(statuses)
            );
        }

        public async Task<List<StatusPreviewDto>> GetLatestStatusPreviews(long assetId)
        {
            var statuses = await _statusRepository
                .GetAll()
                .Include(s => s.Package)
                .ThenInclude(p => p.Asset)
                .ThenInclude(a => a.Group.Parent.Parent)
                .Where(s => s.Package.Asset.Id == assetId)
                .GroupBy(s => s.Package)
                .OrderBy(p => p.Key.Name)
                .Select(g => g.OrderByDescending(s => s.Time).FirstOrDefault())
                .ToListAsync();

            return _objectMapper.Map<List<StatusPreviewDto>>(statuses);
        }
    }
}
