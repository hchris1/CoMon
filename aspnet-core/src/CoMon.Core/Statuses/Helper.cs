using Abp.Domain.Repositories;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Statuses
{
    public static class Helper
    {
        public static async Task<bool> IsLatest(IRepository<Status, long> statusRepository, Status status)
        {
            var latestId = await statusRepository
                .GetAll()
                .Where(s => s.Package.Id == status.Package.Id)
                .OrderByDescending(s => s.Time)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
            return latestId == status.Id;
        }

        public static async Task<StatusDto> AddKpiStatistics(IRepository<Status, long> statusRepository, StatusDto status)
        {
            foreach (var kpi in status.KPIs)
            {
                var cutOff = DateTime.UtcNow - TimeSpan.FromDays(30);
                var query = statusRepository
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
    }
}
