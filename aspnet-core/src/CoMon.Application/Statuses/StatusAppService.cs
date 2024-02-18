
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.ObjectMapping;
using CoMon.Assets;
using CoMon.Assets.Dtos;
using CoMon.Groups;
using CoMon.Groups.Dtos;
using CoMon.Packages;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Statuses
{
    [AbpAuthorize]
    public class StatusAppService(IRepository<Asset, long> assetRepository, IRepository<Group, long> groupRepository,
        IRepository<Status, long> statusRepository, IRepository<Package, long> packageRepository, IObjectMapper objectMapper) : CoMonAppServiceBase
    {
        private readonly IRepository<Asset, long> _assetRepository = assetRepository;
        private readonly IRepository<Group, long> _groupRepository = groupRepository;
        private readonly IRepository<Status, long> _statusRepository = statusRepository;
        private readonly IRepository<Package, long> _packageRepository = packageRepository;
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
                .AsSplitQuery()
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Status not found.");

            status.IsLatest = await IsLatest(status);

            var statusDto = _objectMapper.Map<StatusDto>(status);
            return await AddKpiStatistics(statusDto);
        }

        private async Task<StatusDto> AddKpiStatistics(StatusDto status)
        {
            foreach (var kpi in status.KPIs)
            {
                var cutOff = DateTime.UtcNow - TimeSpan.FromDays(30);
                var query = _statusRepository
                    .GetAll()
                    .Include(s => s.Package)
                    .Include(s => s.KPIs)
                    .Select(s => new { s.Time, s.KPIs, s.PackageId })
                    .Where(s => s.Time >= cutOff)
                    .Where(s => s.PackageId == status.Package.Id)
                    .Select(s => s.KPIs)
                    .SelectMany(x => x)
                    .Where(k => k.Name == kpi.Name)
                    .Select(k => k.Value)
                    .Where(v => v.HasValue);

                kpi.ThirtyDayAverage = await query.AverageAsync();
                kpi.ThirtyDayMax = await query.MaxAsync();
                kpi.ThirtyDayMin = await query.MinAsync();
            }
            return status;
        }

        public async Task<StatusPreviewDto> GetPreview(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.Package)
                .ThenInclude(p => p.Asset)
                .ThenInclude(a => a.Group.Parent.Parent)
                .AsSplitQuery()
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Status not found.");
            status.IsLatest = await IsLatest(status);
            return _objectMapper.Map<StatusPreviewDto>(status);
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
                .AsSplitQuery()
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Status not found.");

            var latestStatus = await _statusRepository
                .GetAll()
                .Where(s => s.Package.Id == status.Package.Id)
                .OrderByDescending(s => s.Time)
                .FirstOrDefaultAsync();

            if (latestStatus.Id == id)
                latestStatus = null;

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
                .AsSplitQuery()
                .ToListAsync();

            var groups = await _groupRepository
                .GetAll()
                .Include(g => g.Parent.Parent)
                .AsSplitQuery()
                .ToListAsync();

            return new StatusTableOptionsDto()
            {
                Assets = _objectMapper.Map<List<AssetPreviewDto>>(assets),
                Groups = _objectMapper.Map<List<GroupPreviewDto>>(groups),
            };
        }

        public async Task<PagedResultDto<StatusPreviewDto>> GetStatusTable(PagedResultRequestDto request, long? assetId, long? groupId, long? packageId, Criticality? criticality, bool latestOnly = true)
        {
            var latestStatusIds = await _packageRepository
                .GetAll()
                .Select(p => p.Statuses.OrderByDescending(s => s.Time).FirstOrDefault())
                .Where(s => s != null)
                .Select(s => s.Id)
                .ToListAsync();

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
                // Thanks Copilot for this awesome fix ;( Nobody will create deeper group structures anyway... Right?
                // TODO: Fix this
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
                query = query.Where(s => latestStatusIds.Contains(s.Id));

            if (criticality != null)
                query = query.Where(s => s.Criticality == criticality);

            var statuses = query
                .OrderByDescending(s => s.Time)
                .Skip(request.SkipCount)
                .Take(request.MaxResultCount)
                .AsSplitQuery()
                .ToList();

            foreach (var status in statuses)
                status.IsLatest = latestStatusIds.Contains(status.Id);

            return new PagedResultDto<StatusPreviewDto>(
                query.Count(),
                _objectMapper.Map<List<StatusPreviewDto>>(statuses)
            );
        }

        public async Task<StatusPreviewDto> GetLatestStatusPreview(long packageId)
        {
            var status = await _statusRepository
                .GetAll()
                .Include(s => s.Package)
                .ThenInclude(p => p.Asset)
                .ThenInclude(a => a.Group.Parent.Parent)
                .Where(s => s.Package.Id == packageId)
                .GroupBy(s => s.Package)
                .OrderBy(p => p.Key.Name)
                .Select(g => g.OrderByDescending(s => s.Time).FirstOrDefault())
                .AsSplitQuery()
                .SingleOrDefaultAsync();

            return _objectMapper.Map<StatusPreviewDto>(status);
        }

        public void DeleteAll()
        {
            var query = _statusRepository.GetAll().Where(s => s.Criticality != null);
            _statusRepository.RemoveRange(query);
        }
    }
}
