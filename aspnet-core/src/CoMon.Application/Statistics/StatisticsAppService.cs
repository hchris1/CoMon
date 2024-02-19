using Abp.Domain.Repositories;
using CoMon.Assets;
using CoMon.Authorization.Users;
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
        IRepository<Package, long> packageRepository, IRepository<Status, long> statusRepository, IRepository<User, long> userRepository) : CoMonAppServiceBase
    {
        private readonly IRepository<Asset, long> _assetRepository = assetRepository;
        private readonly IRepository<Dashboard, long> _dashboardRepository = dashboardRepository;
        private readonly IRepository<Group, long> _groupRepository = groupRepository;
        private readonly IRepository<Package, long> _packageRepository = packageRepository;
        private readonly IRepository<Status, long> _statusRepository = statusRepository;
        private readonly IRepository<User, long> _userRepository = userRepository;

        public async Task<StatisticsDto> Get()
        {
            return new StatisticsDto()
            {
                AssetCount = await _assetRepository.CountAsync(),
                // Add 1 for the predefined dashboard
                CustomDashboardCount = await _dashboardRepository.CountAsync() + 1,
                GroupCount = await _groupRepository.CountAsync(),
                PackageCount = await _packageRepository.CountAsync(),
                StatusCount = (await _statusRepository.GetAll().OrderByDescending(s => s.Time).FirstOrDefaultAsync())?.Id ?? 0,
                UserCount = await _userRepository.CountAsync()
            };
        }
    }
}
