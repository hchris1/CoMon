using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using CoMon.Assets;
using CoMon.Groups;
using CoMon.Packages;
using CoMon.Statuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx.Synchronous;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Notifications
{
    public class HaSender(ILogger<HaSender> logger, IRepository<Package, long> packageRepository, IRepository<Group, long> groupRepository,
        IRepository<Asset, long> assetRepository, IUnitOfWorkManager unitOfWorkManager, MqttClient mqttClient, IConfiguration configuration) : ISingletonDependency, IShouldInitialize
    {
        private readonly ILogger<HaSender> _logger = logger;
        private readonly IRepository<Package, long> _packageRepository = packageRepository;
        private readonly IRepository<Group, long> _groupRepository = groupRepository;
        private readonly IRepository<Asset, long> _assetRepository = assetRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager = unitOfWorkManager;
        private readonly MqttClient _mqttClient = mqttClient;
        private readonly IConfiguration _configuration = configuration;
        private bool _enabled = false;

        public void Initialize()
        {
            try
            {
                var section = _configuration.GetSection("HomeAssistant");
                _enabled = bool.Parse(section["Enabled"]);

                if (!_enabled)
                {
                    _logger.LogInformation("HaSender is disabled and will remain inactive.");
                    return;
                }

                _mqttClient.Connect().WaitAndUnwrapException();
                SendPackageCountsByCriticality().WaitAndUnwrapException();
                SendAssetAndPackageUpdates().WaitAndUnwrapException();
                SendEntityCounts().WaitAndUnwrapException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to initialize HaSender. HaSender will remain disabled.");
            }
        }

        public async Task ProcessStatusUpdate(StatusUpdateDto update)
        {
            try
            {
                if (!_enabled) return;

                await SendStatusUpdate(update);
                await SendPackageCountsByCriticality();
                await SendAssetStatus(update.AssetId);
                await SendEntityCounts();
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error when processing status update in HaSender.");
            }
        }

        private async Task SendStatusUpdate(StatusUpdateDto update)
        {
            var sensor = HaHelper.CreateStatusSensor(_mqttClient,
                HaHelper.CreatePackageName(update.PackageId, update.PackageName),
                HaHelper.CreatePackageIdentifier(update.PackageId), update.Criticality);

            await sensor.Announce();
            await sensor.PublishState();
        }

        private async Task SendAssetUpdate(Asset asset, Criticality? criticality)
        {
            var sensor = HaHelper.CreateStatusSensor(_mqttClient,
                HaHelper.CreateAssetName(asset.Id, asset.Name),
                HaHelper.CreateAssetIdentifier(asset.Id), criticality);

            await sensor.Announce();
            await sensor.PublishState();
        }

        private async Task SendPackageUpdate(Package package, Criticality? criticality)
        {
            var sensor = HaHelper.CreateStatusSensor(_mqttClient,
                HaHelper.CreatePackageName(package.Id, package.Name),
                HaHelper.CreatePackageIdentifier(package.Id), criticality);

            await sensor.Announce();
            await sensor.PublishState();
        }

        private async Task SendPackageCountsByCriticality()
        {
            using var uow = _unitOfWorkManager.Begin();
            var packages = await _packageRepository
                .GetAll()
                .Include(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .AsSplitQuery()
                .ToListAsync();
            uow.Complete();

            var healthyPackageCount = packages.Where(p => p.Statuses.FirstOrDefault()?.Criticality == Criticality.Healthy).Count();
            var warningPackageCount = packages.Where(p => p.Statuses.FirstOrDefault()?.Criticality == Criticality.Warning).Count();
            var alertPackageCount = packages.Where(p => p.Statuses.FirstOrDefault()?.Criticality == Criticality.Alert).Count();
            var unknownPackageCount = packages.Where(p => p.Statuses.FirstOrDefault()?.Criticality == null).Count();

            var healthyCountSensor = HaHelper.CreateCountSensor(_mqttClient, "Healthy Packages Count", "comon-healthy-package-count", healthyPackageCount);
            await healthyCountSensor.Announce();
            await healthyCountSensor.PublishState();

            var warningCountSensor = HaHelper.CreateCountSensor(_mqttClient, "Warning Packages Count", "comon-warning-package-count", warningPackageCount);
            await warningCountSensor.Announce();
            await warningCountSensor.PublishState();

            var alertCountSensor = HaHelper.CreateCountSensor(_mqttClient, "Alert Packages Count", "comon-alert-package-count", alertPackageCount);
            await alertCountSensor.Announce();
            await alertCountSensor.PublishState();

            var unknownCountSensor = HaHelper.CreateCountSensor(_mqttClient, "Unknown Packages Count", "comon-unknown-package-count", unknownPackageCount);
            await unknownCountSensor.Announce();
            await unknownCountSensor.PublishState();
        }

        private async Task SendAssetAndPackageUpdates()
        {
            using var uow = _unitOfWorkManager.Begin();
            var assets = await _assetRepository
                .GetAll()
                .Include(a => a.Packages)
                .ThenInclude(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .AsSplitQuery()
                .ToListAsync();
            uow.Complete();

            foreach (var asset in assets)
            {
                var worstCrit = asset.Packages.SelectMany(p => p.Statuses).Select(s => s.Criticality).Max();
                await SendAssetUpdate(asset, worstCrit);
                foreach (var package in asset.Packages)
                {
                    await SendPackageUpdate(package, package.Statuses.Select(s => s.Criticality).Max());
                }
            }
        }

        private async Task SendAssetStatus(long id)
        {
            using var uow = _unitOfWorkManager.Begin();
            var asset = await _assetRepository
                .GetAll()
                .Where(a => a.Id == id)
                .Include(a => a.Packages)
                .ThenInclude(p => p.Statuses
                    .OrderByDescending(s => s.Time)
                    .Take(1))
                .AsSplitQuery()
                .SingleOrDefaultAsync();
            uow.Complete();

            var worstCriticality = asset.Packages.SelectMany(p => p.Statuses).Select(s => s.Criticality).Max();

            await SendAssetUpdate(asset, worstCriticality);
        }

        private async Task SendEntityCounts()
        {
            using var uow = _unitOfWorkManager.Begin();
            var groupCount = await _groupRepository.CountAsync();
            var assetCount = await _assetRepository.CountAsync();
            var packageCount = await _packageRepository.CountAsync();
            uow.Complete();

            var groupCountSensor = HaHelper.CreateCountSensor(_mqttClient, "Group Count", "comon-group-count", groupCount);
            await groupCountSensor.Announce();
            await groupCountSensor.PublishState();

            var assetCountSensor = HaHelper.CreateCountSensor(_mqttClient, "Asset Count", "comon-asset-count", assetCount);
            await assetCountSensor.Announce();
            await assetCountSensor.PublishState();

            var packageCountSensor = HaHelper.CreateCountSensor(_mqttClient, "Package Count", "comon-package-count", packageCount);
            await packageCountSensor.Announce();
            await packageCountSensor.PublishState();
        }
    }
}
