using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Validation;
using CoMon.Assets;
using CoMon.Assets.Dtos;
using CoMon.Dashboards.Dtos;
using CoMon.Groups;
using CoMon.Groups.Dtos;
using CoMon.Packages;
using CoMon.Packages.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Dashboards
{
    [AbpAuthorize]
    public class DashboardAppService(IObjectMapper mapper, IRepository<Asset, long> assetRepository,
        IRepository<Dashboard, long> dashboardRepository, IRepository<Group, long> groupRepository,
        IRepository<Package, long> packageRepository) : CoMonAppServiceBase
    {
        private readonly IObjectMapper _mapper = mapper;
        private readonly IRepository<Asset, long> _assetRepository = assetRepository;
        private readonly IRepository<Dashboard, long> _dashboardRepository = dashboardRepository;
        private readonly IRepository<Group, long> _groupRepository = groupRepository;
        private readonly IRepository<Package, long> _packageRepository = packageRepository;

        public async Task<DashboardDto> Get(long id)
        {
            var dashboard = await _dashboardRepository
                .GetAll()
                .Where(d => d.Id == id)
                .Include(d => d.Tiles)
                .AsSplitQuery()
                .AsNoTracking()
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Dashboard not found for given id.");

            return _mapper.Map<DashboardDto>(dashboard);
        }

        public async Task<List<DashboardPreviewDto>> GetAllPreviews()
        {
            var dashboards = await _dashboardRepository
                .GetAll()
                .Include(d => d.Tiles)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();

            return dashboards.Select(d => new DashboardPreviewDto()
            {
                Id = d.Id,
                Name = d.Name,
                GroupCount = d.Tiles.Where(t => t.ItemType == DashboardTileType.Group).Count(),
                AssetCount = d.Tiles.Where(t => t.ItemType == DashboardTileType.Asset).Count(),
                PackageCount = d.Tiles.Where(t => t.ItemType == DashboardTileType.Package).Count(),
            }).ToList();
        }

        public async Task<long> Create(string name)
        {
            var dashboard = new Dashboard { Name = name?.Trim() };

            if (string.IsNullOrWhiteSpace(dashboard.Name))
                throw new AbpValidationException("Dashboard name may not be empty.");

            return await _dashboardRepository.InsertAndGetIdAsync(dashboard);
        }

        public async Task Delete(long id)
        {
            await _dashboardRepository.DeleteAsync(id);
        }

        public async Task UpdateName(long id, string name)
        {
            var dashboard = await _dashboardRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Dashboard not found for given id.");

            dashboard.Name = name?.Trim();

            if (string.IsNullOrWhiteSpace(dashboard.Name))
                throw new AbpValidationException("Dashboard name may not be empty.");

            await _dashboardRepository.UpdateAsync(dashboard);
        }

        public async Task UpdateTilePositions(long id, List<UpdateTilePositionDto> positionDtos)
        {
            var dashboard = await _dashboardRepository
                .GetAll()
                .Where(d => d.Id == id)
                .Include(d => d.Tiles)
                .AsSplitQuery()
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Dashboard not found for given id.");

            foreach (var positionDto in positionDtos)
            {
                var tile = dashboard.Tiles
                    .Where(t => t.Id == positionDto.TileId)
                    .SingleOrDefault()
                    ?? throw new EntityNotFoundException("Tile not found for given tile id.");

                tile.X = positionDto.X;
                tile.Y = positionDto.Y;
                tile.Width = positionDto.Width;
                tile.Height = positionDto.Height;
            }

            await _dashboardRepository.UpdateAsync(dashboard);
        }

        public async Task UpdateTileContent(long id, long tileId, string content)
        {
            var dashboard = await _dashboardRepository
                .GetAll()
                .Where(d => d.Id == id)
                .Include(d => d.Tiles)
                .AsSplitQuery()
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Dashboard not found for given id.");

            var tile = dashboard.Tiles
                .Where(t => t.Id == tileId)
                .SingleOrDefault()
                ?? throw new EntityNotFoundException("Tile not found for given tile id.");

            tile.Content = content;

            await _dashboardRepository.UpdateAsync(dashboard);
        }

        public async Task AddTile(long id, CreateDashboardTileDto tileDto)
        {
            var dashboard = await _dashboardRepository
                .GetAll()
                .Where(d => d.Id == id)
                .Include(d => d.Tiles)
                .AsSplitQuery()
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Dashboard not found for given id.");

            var tile = _mapper.Map<DashboardTile>(tileDto);
            tile.Y = dashboard.Tiles.Count > 0 ? dashboard.Tiles.Max(t => t.Y + t.Height) + 1 : 0;

            // Verify that the group/asset/package exists
            switch (tileDto.ItemType)
            {
                case DashboardTileType.Group:
                    await _groupRepository.GetAsync(tileDto.ItemId);
                    break;

                case DashboardTileType.Asset:
                    await _assetRepository.GetAsync(tileDto.ItemId);
                    break;

                case DashboardTileType.Package:
                    await _packageRepository.GetAsync(tileDto.ItemId);
                    break;

                case DashboardTileType.Markdown:
                    break;

                default:
                    throw new AbpValidationException("Invalid item type.");
            }


            dashboard.Tiles.Add(tile);
            await _dashboardRepository.UpdateAsync(dashboard);
        }

        public async Task DeleteTile(long id, long tileId)
        {
            var dashboard = await _dashboardRepository
                .GetAll()
                .Where(d => d.Id == id)
                .Include(d => d.Tiles)
                .AsSplitQuery()
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Dashboard not found for given id.");

            var tileToRemove = dashboard.Tiles
                .Where(t => t.Id == tileId)
                .SingleOrDefault()
                ?? throw new EntityNotFoundException("Tile not found for given tile id.");

            dashboard.Tiles.Remove(tileToRemove);
            await _dashboardRepository.UpdateAsync(dashboard);
        }

        public async Task<DashboardTileOptionDto> GetDashboardTileOptions()
        {
            return new DashboardTileOptionDto()
            {
                Assets = _mapper.Map<List<AssetPreviewDto>>(await _assetRepository
                    .GetAll()
                    .OrderBy(g => g.Name)
                    .Include(a => a.Group.Parent.Parent)
                    .AsSplitQuery()
                    .ToListAsync()),
                Groups = _mapper.Map<List<GroupPreviewDto>>(await _groupRepository
                    .GetAll()
                    .OrderBy(g => g.Name)
                    .Include(g => g.Parent.Parent)
                    .AsSplitQuery()
                    .ToListAsync()),
                Packages = _mapper.Map<List<PackagePreviewDto>>(await _packageRepository
                    .GetAll()
                    .OrderBy(g => g.Name)
                    .Include(p => p.Asset)
                    .ThenInclude(a => a.Group.Parent.Parent)
                    .AsSplitQuery()
                    .ToListAsync())
            };
        }
    }
}
