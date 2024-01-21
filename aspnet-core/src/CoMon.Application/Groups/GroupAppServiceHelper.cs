using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Assets;
using CoMon.Statuses;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Groups
{
    public static class GroupAppServiceHelper
    {
        public static async Task<List<Status>> GetLatestStatusesFromGroup(IRepository<Group, long> groupRepository, long groupId)
        {
            var group = await groupRepository
                .GetAll()
                .Where(g => g.Id == groupId)
                .Include(g => g.SubGroups)
                .Include(g => g.Assets)
                .ThenInclude(a => a.Packages)
                .ThenInclude(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .AsSplitQuery()
                .SingleOrDefaultAsync() ?? throw new EntityNotFoundException("Group not found");

            var statuses = group.Assets.Select(a => a.Packages).SelectMany(x => x).Select(p => p.Statuses).SelectMany(x => x).ToList();

            foreach (var subGroup in group.SubGroups)
                statuses.AddRange(await GetLatestStatusesFromGroup(groupRepository, subGroup.Id));

            return statuses;
        }

        public static async Task<List<Status>> GetAllLatestStatuses(IRepository<Group, long> groupRepository, IRepository<Asset, long> assetRepository)
        {
            var groupIds = await groupRepository
                    .GetAll()
                    .OrderBy(g => g.Name)
                    .Where(g => g.Parent == null)
                    .Select(g => g.Id)
                    .ToListAsync();

            var statuses = groupIds
                .Select(async id => await GetLatestStatusesFromGroup(groupRepository, id))
                .Select(t => t.Result)
                .SelectMany(x => x)
                .ToList();

            var assets = await assetRepository
                .GetAll()
                .Where(a => a.Group == null)
                .Include(a => a.Packages)
                .ThenInclude(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .AsSplitQuery()
                .ToListAsync();

            statuses.AddRange(assets.Select(a => a.Packages).SelectMany(x => x).Select(p => p.Statuses).SelectMany(x => x).ToList());

            return statuses;
        }
    }
}
