using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Statuses;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Assistant.Plugins
{
    [Description("Plugin to get information about the statuses produced by packages.")]
    public class StatusPlugin(IRepository<Status, long> _statusRepository, IObjectMapper _mapper)
    {
        [KernelFunction, Description("Get a status by its ID.")]
        public async Task<StatusDto> GetStatusById(int id)
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
                .AsNoTracking()
                .FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Status not found.");

            status.IsLatest = await Helper.IsLatest(_statusRepository, status);

            var statusDto = _mapper.Map<StatusDto>(status);
            return await Helper.AddKpiStatistics(_statusRepository, statusDto);
        }
    }
}
