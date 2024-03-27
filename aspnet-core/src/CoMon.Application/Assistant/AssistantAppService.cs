using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.UI;
using CoMon.Assets;
using CoMon.Groups;
using CoMon.Statuses;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoMon.Assistant
{
    public class AssistantAppService(Assistant assistant, IRepository<Group, long> groupRepository, IRepository<Asset, long> assetRepository, IRepository<Status, long> statusRepository, IObjectMapper objectMapper)
        : CoMonAppServiceBase
    {
        private readonly Assistant _assistant = assistant;
        private readonly IRepository<Group, long> _groupRepository = groupRepository;
        private readonly IRepository<Asset, long> _assetRepository = assetRepository;
        private readonly IRepository<Status, long> _statusRepository = statusRepository;
        private readonly IObjectMapper _objectMapper = objectMapper;

        public async Task<string> GetRecommendations(long statusId)
        {
            try
            {
                var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == statusId)
                .Include(s => s.Package)
                .ThenInclude(s => s.Asset)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync() 
                    ?? throw new EntityNotFoundException("Status not found.");

                var stringified = JsonSerializer.Serialize(_objectMapper.Map<StatusPreviewDto>(status));

                return await _assistant.GetRecommendations(stringified);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public async Task<string> GetAssetSummary(long assetId)
        {
            try
            {
                var asset = await _assetRepository
                .GetAll()
                .Where(a => a.Id == assetId)
                .Include(a => a.Packages)
                .ThenInclude(p => p.Statuses
                        .OrderByDescending(s => s.Time)
                        .Take(1))
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Asset not found.");

                var statuses = asset.Packages
                    .Select(p => p.Statuses)
                    .SelectMany(x => x)
                    .ToList();

                var previews = _objectMapper.Map<List<StatusPreviewDto>>(statuses);

                var stringified = JsonSerializer.Serialize(previews);

                return await _assistant.GetSummary(stringified);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<string> GetGroupSummary(long? groupId)
        {
            try
            {
                List<Status> statuses;
                if (groupId.HasValue)
                    statuses = await GroupAppServiceHelper.GetLatestStatusesFromGroup(_groupRepository, groupId.Value);
                else
                    statuses = await GroupAppServiceHelper.GetAllLatestStatuses(_groupRepository, _assetRepository);

                var previews = _objectMapper.Map<List<StatusPreviewDto>>(statuses);

                var input = JsonSerializer.Serialize(previews);
                return await _assistant.GetSummary(input);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
