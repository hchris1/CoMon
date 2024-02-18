using Abp.Domain.Repositories;
using CoMon.Assets;
using CoMon.Dashboards;
using CoMon.Groups;
using CoMon.Packages;
using CoMon.Statistics.Dtos;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Statistics
{
    public class StatisticsAppService(
        IRepository<Asset, long> assetRepository, IRepository<Dashboard, long> dashboardRepository, IRepository<Group, long> groupRepository,
        IRepository<Package, long> packageRepository, IRepository<Status, long> statusRepository) : CoMonAppServiceBase
    {
        private readonly IRepository<Asset, long> _assetRepository = assetRepository;
        private readonly IRepository<Dashboard, long> _dashboardRepository = dashboardRepository;
        private readonly IRepository<Group, long> _groupRepository = groupRepository;
        private readonly IRepository<Package, long> _packageRepository = packageRepository;
        private readonly IRepository<Status, long> _statusRepository = statusRepository;

        public async Task<StatisticsDto> Get()
        {
            return new StatisticsDto()
            {
                AssetCount = await _assetRepository.CountAsync(),
                CustomDashboardCount = await _dashboardRepository.CountAsync(),
                GroupCount = await _groupRepository.CountAsync(),
                StatusCount = (await _statusRepository.GetAll().OrderByDescending(s => s.Time).FirstOrDefaultAsync())?.Id ?? 0,
                PackageCount = await _packageRepository.CountAsync()
            };
        }
    }
}
