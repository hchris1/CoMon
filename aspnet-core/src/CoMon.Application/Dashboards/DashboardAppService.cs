﻿using Abp.Domain.Entities;
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
using CoMon.Statuses;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Dashboards
{
    public class DashboardAppService : CoMonAppServiceBase
    {
        private readonly IObjectMapper _mapper;
        private readonly IRepository<Asset, long> _assetRepository;
        private readonly IRepository<Dashboard, long> _dashboardRepository;
        private readonly IRepository<Group, long> _groupRepository;
        private readonly IRepository<Package, long> _packageRepository;
        private readonly IRepository<Status, long> _statusRepository;

        public DashboardAppService(IObjectMapper mapper, IRepository<Asset, long> assetRepository,
            IRepository<Dashboard, long> dashboardRepository, IRepository<Group, long> groupRepository,
            IRepository<Package, long> packageRepository, IRepository<Status, long> statusRepository)
        {
            _mapper = mapper;
            _assetRepository = assetRepository;
            _dashboardRepository = dashboardRepository;
            _groupRepository = groupRepository;
            _packageRepository = packageRepository;
            _statusRepository = statusRepository;
        }

        public async Task<List<DashboardDto>> GetAll()
        {
            var dashboards = await _dashboardRepository
                .GetAll()
                .Include(d => d.Tiles)
                .ToListAsync();

            return _mapper.Map<List<DashboardDto>>(dashboards);
        }

        public async Task<DashboardDto> Get(long id)
        {
            var dashboard = await _dashboardRepository
                .GetAll()
                .Where(d => d.Id == id)
                .Include(d => d.Tiles)
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Dashboard not found for given id.");

            return _mapper.Map<DashboardDto>(dashboard);
        }

        public async Task<long> Create(string name)
        {
            var dashboard = new Dashboard { Name = name.Trim() };

            return await _dashboardRepository.InsertAndGetIdAsync(dashboard);
        }

        public async Task Delete(long id)
        {
            await _dashboardRepository.DeleteAsync(id);
        }

        public async Task UpdateName(long id, string name)
        {
            var dashboard = await _dashboardRepository
                .GetAll()
                .Where(d => d.Id == id)
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Dashboard not found for given id.");

            dashboard.Name = name.Trim();

            await _dashboardRepository.UpdateAsync(dashboard);
        }

        public async Task AddTile(long id, CreateDashboardTileDto tileDto)
        {
            var dashboard = await _dashboardRepository
                .GetAll()
                .Where(d => d.Id == id)
                .Include(d => d.Tiles)
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Dashboard not found for given id.");

            var tile = _mapper.Map<DashboardTile>(tileDto);
            tile.SortIndex = dashboard.Tiles.Select(t => t.SortIndex).DefaultIfEmpty(-1).Max() + 1;

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
                    .ToListAsync()),
                Groups = _mapper.Map<List<GroupPreviewDto>>(await _groupRepository
                    .GetAll()
                    .OrderBy(g => g.Name)
                    .Include(g => g.Parent.Parent)
                    .ToListAsync()),
                Packages = _mapper.Map<List<PackagePreviewDto>>(await _packageRepository
                    .GetAll()
                    .Include(p => p.Asset)
                    .ThenInclude(a => a.Group.Parent.Parent)
                    .ToListAsync())
            };
        }

        public async Task MoveTileUp(long id, long tileId)
        {
            var dashboard = await _dashboardRepository
                .GetAll()
                .Where(d => d.Id == id)
                .Include(d => d.Tiles)
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Dashboard not found for given id.");

            var tileToMoveUp = dashboard.Tiles
                .Where(t => t.Id == tileId)
                .SingleOrDefault()
                ?? throw new EntityNotFoundException("Tile not found for given tile id.");

            var originalSortIndex = tileToMoveUp.SortIndex;

            var tileToMoveDown = dashboard.Tiles
                .OrderByDescending(t => t.Id)
                .Where(t => t.SortIndex < tileToMoveUp.SortIndex && t.Id != tileToMoveUp.Id)
                .FirstOrDefault();

            tileToMoveUp.SortIndex = tileToMoveDown?.SortIndex
                ?? throw new AbpValidationException("Can't move the tile further down.");

            if (tileToMoveDown != null)
                tileToMoveDown.SortIndex = originalSortIndex;

            await _dashboardRepository.UpdateAsync(dashboard);
        }

        public async Task MoveTileDown(long id, long tileId)
        {
            var dashboard = await _dashboardRepository
                .GetAll()
                .Where(d => d.Id == id)
                .Include(d => d.Tiles)
                .SingleOrDefaultAsync()
                ?? throw new EntityNotFoundException("Dashboard not found for given id.");

            var tileToMoveDown = dashboard.Tiles
                .Where(t => t.Id == tileId)
                .SingleOrDefault()
                ?? throw new EntityNotFoundException("Tile not found for given tile id.");

            var originalSortIndex = tileToMoveDown.SortIndex;

            var tileToMoveUp = dashboard.Tiles
                .OrderBy(t => t.SortIndex)
                .Where(t => t.SortIndex > tileToMoveDown.SortIndex && t.Id != tileToMoveDown.Id)
                .FirstOrDefault();

            tileToMoveDown.SortIndex = tileToMoveUp?.SortIndex
                ?? throw new AbpValidationException("Can't move the tile further down.");

            if (tileToMoveUp != null)
                tileToMoveUp.SortIndex = originalSortIndex;


            await _dashboardRepository.UpdateAsync(dashboard);
        }
    }
}
