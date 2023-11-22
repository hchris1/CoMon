
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Assets;
using CoMon.Assets.Dtos;
using CoMon.Groups;
using CoMon.Groups.Dtos;
using CoMon.Statuses.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoMon.Statuses
{
    public class StatusAppService : CoMonAppServiceBase, IStatusAppService
    {
        private readonly IRepository<Asset, long> _assetRepository;
        private readonly IRepository<Group, long> _groupRepository;
        private readonly IRepository<Status, long> _statusRepository;
        private readonly IObjectMapper _objectMapper;

        public StatusAppService(IRepository<Asset, long> assetRepository, IRepository<Group, long> groupRepository,
            IRepository<Status, long> statusRepository, IObjectMapper objectMapper)
        {
            _assetRepository = assetRepository;
            _groupRepository = groupRepository;
            _statusRepository = statusRepository;
            _objectMapper = objectMapper;
        }

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
                .FirstOrDefaultAsync() 
                ?? throw new EntityNotFoundException("Status not found.");

            status.IsLatest = await IsLatest(status);

            return _objectMapper.Map<StatusDto>(status);
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
                .FirstOrDefaultAsync() 
                ?? throw new EntityNotFoundException("Status not found.");

            var latestStatus = await _statusRepository
                .GetAll()
                .Where(s => s.Package.Id == status.Package.Id)
                .OrderByDescending(s => s.Time)
                .FirstOrDefaultAsync();

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

            if (status.Id == latestStatus.Id)
                latestStatus = null;


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
                .ToListAsync();

            var groups = await _groupRepository
                .GetAll()
                .Include(g => g.Parent.Parent)
                .ToListAsync();

            return new StatusTableOptionsDto()
            {
                Assets = _objectMapper.Map<List<AssetPreviewDto>>(assets),
                Groups = _objectMapper.Map<List<GroupPreviewDto>>(groups),
            };
        }

        public async Task<PagedResultDto<StatusPreviewDto>> GetStatusTable(PagedResultRequestDto request, long? assetId, long? groupId, long? packageId, Criticality? criticality, bool latestOnly = true)
        {
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
                // WTH is this??
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
            {
                var latestStatuses = await query.GroupBy(s => s.Package).Select(g => g.OrderByDescending(s => s.Time).FirstOrDefault()).ToListAsync();
                query = latestStatuses.AsQueryable();
            }

            if (criticality != null)
                query = query.Where(s => s.Criticality == criticality);

            var statuses = query
                .OrderByDescending(s => s.Time)
                .Skip(request.SkipCount)
                .Take(request.MaxResultCount)
                .ToList();

            var totalCount = query.Count();

            if (!latestOnly)
            {
                foreach (var status in statuses)
                    status.IsLatest = await IsLatest(status);
            }

            return new PagedResultDto<StatusPreviewDto>(
                totalCount,
                _objectMapper.Map<List<StatusPreviewDto>>(statuses)
            );
        }

        public async Task<List<StatusPreviewDto>> GetLatestStatusPreviews(long assetId)
        {
            var statuses = await _statusRepository
                .GetAll()
                .Include(s => s.Package)
                .ThenInclude(p => p.Asset)
                .ThenInclude(a => a.Group.Parent.Parent)
                .Where(s => s.Package.Asset.Id == assetId)
                .GroupBy(s => s.Package)
                .OrderBy(p => p.Key.Name)
                .Select(g => g.OrderByDescending(s => s.Time).FirstOrDefault())
                .ToListAsync();

            return _objectMapper.Map<List<StatusPreviewDto>>(statuses);
        }

        // TODO: REMOVE
        public async Task InsertSampleLineCharts(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var timeBasedChart = new Chart
            {
                Title = "Time Based Line Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.Line,
                Labels = new(),
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(5),
                                Y = new List<double>() { 3 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(3),
                                Y = new List<double>() { 5 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(1),
                                Y = new List<double>() { 4 }
                            }
                        }
                    }
                }
            };

            var valueBasedChart = new Chart
            {
                Title = "Time Based Line Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.Line,
                Labels = new(),
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        XUnit = "Iteration",
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                X = 3,
                                Y = new List<double>() { 3 }
                            },
                            new DataPoint()
                            {
                                X = 4,
                                Y = new List<double>() { 5 }
                            },
                            new DataPoint()
                            {
                                X = 7,
                                Y = new List<double>() { 4 }
                            }
                        }
                    }
                }
            };

            status.Charts.Add(valueBasedChart);
            status.Charts.Add(timeBasedChart);

            _statusRepository.Update(status);
        }

        public async Task InsertSampleAreaCharts(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var timeBasedChart = new Chart
            {
                Title = "Time Based Area Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.Area,
                Labels = new(),
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(5),
                                Y = new List<double>() { 3 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(3),
                                Y = new List<double>() { 5 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(1),
                                Y = new List<double>() { 4 }
                            }
                        }
                    },
                    new Series
                    {
                        Name = "Second Series",
                        VizType = VizType.Secondary,
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(6),
                                Y = new List<double>() { 8 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(4),
                                Y = new List<double>() { 13 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(2),
                                Y = new List<double>() { 11 }
                            }
                        }
                    }
                }
            };

            var valueBasedChart = new Chart
            {
                Title = "Value Based Area Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.Area,
                Labels = new(),
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        XUnit = "Iteration",
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                X = 3,
                                Y = new List<double>() { 3 }
                            },
                            new DataPoint()
                            {
                                X = 4,
                                Y = new List<double>() { 5 }
                            },
                            new DataPoint()
                            {
                                X = 7,
                                Y = new List<double>() { 4 }
                            }
                        }
                    }
                }
            };

            status.Charts.Add(valueBasedChart);
            status.Charts.Add(timeBasedChart);

            _statusRepository.Update(status);
        }

        public async Task InsertSampleBarCharts(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var timeBasedChart = new Chart
            {
                Title = "Time Based Bar Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.Bar,
                Labels = new(),
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(5),
                                Y = new List<double>() { 3 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(3),
                                Y = new List<double>() { 5 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(1),
                                Y = new List<double>() { 4 }
                            }
                        }
                    },
                    new Series
                    {
                        Name = "Second Series",
                        VizType = VizType.Secondary,
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(6),
                                Y = new List<double>() { 8 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(4),
                                Y = new List<double>() { 13 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(2),
                                Y = new List<double>() { 11 }
                            }
                        }
                    }
                }
            };

            var valueBasedChart = new Chart
            {
                Title = "Value Based Bar Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.Bar,
                Labels = new(),
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        XUnit = "Iteration",
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                X = 3,
                                Y = new List<double>() { 3 }
                            },
                            new DataPoint()
                            {
                                X = 4,
                                Y = new List<double>() { 5 }
                            },
                            new DataPoint()
                            {
                                X = 7,
                                Y = new List<double>() { 4 }
                            }
                        }
                    }
                }
            };

            status.Charts.Add(valueBasedChart);
            status.Charts.Add(timeBasedChart);

            _statusRepository.Update(status);
        }

        public async Task InsertSamplePieCharts(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var pieChart = new Chart
            {
                Title = "Pie Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.Pie,
                Labels = new() { "Label A", "Label B", "Label C" },
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Y = new List<double>() { 30, 15, 17 }
                            },
                        }
                    }
                }
            };

            status.Charts.Add(pieChart);

            _statusRepository.Update(status);
        }

        public async Task InsertSampleDonutCharts(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var pieChart = new Chart
            {
                Title = "Pie Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.Donut,
                Labels = new() { "Label A", "Label B", "Label C" },
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Y = new List<double>() { 30, 15, 17 }
                            },
                        }
                    }
                }
            };

            status.Charts.Add(pieChart);

            _statusRepository.Update(status);
        }

        public async Task InsertSampleRadialBarCharts(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var pieChart = new Chart
            {
                Title = "Radial Bar Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.RadialBar,
                Labels = new() { "Label A", "Label B", "Label C" },
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Y = new List<double>() { 30, 15, 17 }
                            },
                        }
                    }
                }
            };

            status.Charts.Add(pieChart);

            _statusRepository.Update(status);
        }

        public async Task InsertSampleScatterCharts(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var timeBasedChart = new Chart
            {
                Title = "Time Based Scatter Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.Scatter,
                Labels = new(),
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(5),
                                Y = new List<double>() { 3 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(3),
                                Y = new List<double>() { 5 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(1),
                                Y = new List<double>() { 4 }
                            }
                        }
                    },
                    new Series
                    {
                        Name = "Second Series",
                        VizType = VizType.Secondary,
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(6),
                                Y = new List<double>() { 8 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(4),
                                Y = new List<double>() { 13 }
                            },
                            new DataPoint()
                            {
                                Time = DateTime.Now - TimeSpan.FromSeconds(2),
                                Y = new List<double>() { 11 }
                            }
                        }
                    }
                }
            };

            var valueBasedChart = new Chart
            {
                Title = "Value Based Scatter Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.Scatter,
                Labels = new(),
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        XUnit = "Iteration",
                        YUnit = "ms",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                X = 3,
                                Y = new List<double>() { 3 }
                            },
                            new DataPoint()
                            {
                                X = 4,
                                Y = new List<double>() { 5 }
                            },
                            new DataPoint()
                            {
                                X = 7,
                                Y = new List<double>() { 4 }
                            }
                        }
                    }
                }
            };

            status.Charts.Add(valueBasedChart);
            status.Charts.Add(timeBasedChart);

            _statusRepository.Update(status);
        }

        public async Task InsertSampleHeatMapChart(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var heatMapChart = new Chart
            {
                Title = "Heatmap Chart",
                SubTitle = "Some Subtitle",
                Labels = new() { "Label A", "Label B", "Label C" },
                Type = ChartType.HeatMap,
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "Heat 1",
                        VizType = VizType.Primary,
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Y = new List<double>() { 30, 15, 17 }
                            },
                        }
                    },
                    new Series
                    {
                        Name = "Heat 2",
                        VizType = VizType.Danger,
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Y = new List<double>() { 20, 5, 21 }
                            },
                        }
                    }
                }
            };

            status.Charts.Add(heatMapChart);

            _statusRepository.Update(status);
        }

        public async Task InsertSampleRadarChart(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var radarChart = new Chart
            {
                Title = "Radar Chart",
                SubTitle = "Some Subtitle",
                Labels = new() { "Label A", "Label B", "Label C", "Label D", "Label E" },
                Type = ChartType.Radar,
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "Series 1",
                        VizType = VizType.Primary,
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Y = new List<double>() { 30, 15, 17, 21, 5 }
                            },
                        }
                    },
                    new Series
                    {
                        Name = "Series 2",
                        VizType = VizType.Danger,
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Y = new List<double>() { 20, 5, 21, 5, 7 }
                            },
                        }
                    }
                }
            };

            status.Charts.Add(radarChart);

            _statusRepository.Update(status);
        }

        public async Task InsertSamplePolarAreaChart(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var polarAreaChart = new Chart
            {
                Title = "Polar Area Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.PolarArea,
                Labels = new() { "Label A", "Label B", "Label C" },
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "First Series",
                        VizType = VizType.Primary,
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Y = new List<double>() { 30, 15, 17 }
                            },
                        }
                    }
                }
            };

            status.Charts.Add(polarAreaChart);

            _statusRepository.Update(status);
        }

        public async Task InsertSampleRangeAreaChart(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var rangeAreaChart = new Chart
            {
                Title = "Range Area Chart",
                SubTitle = "Some Subtitle",
                Labels = new() { "Label A", "Label B", "Label C", "Label D", "Label E" },
                Type = ChartType.RangeArea,
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "Series 1",
                        VizType = VizType.Primary,
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                X = 1,
                                Y = new List<double>() { 2, 5 }
                            },
                            new DataPoint()
                            {
                                X = 2,
                                Y = new List<double>() { 2, 4 }
                            },
                            new DataPoint()
                            {
                                X = 3,
                                Y = new List<double>() { 3, 6 }
                            },
                            new DataPoint()
                            {
                                X = 4,
                                Y = new List<double>() { 4, 7 }
                            },
                            new DataPoint()
                            {
                                X = 5,
                                Y = new List<double>() { 4, 6 }
                            },
                        }
                    },
                    new Series
                    {
                        Name = "Series 2",
                        VizType = VizType.Warning,
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                X = 1,
                                Y = new List<double>() { 20, 21 }
                            },
                            new DataPoint()
                            {
                                X = 2,
                                Y = new List<double>() {17, 19 }
                            },
                            new DataPoint()
                            {
                                X = 3,
                                Y = new List<double>() { 12, 15 }
                            },
                            new DataPoint()
                            {
                                X = 4,
                                Y = new List<double>() {10, 13 }
                            },
                            new DataPoint()
                            {
                                X = 5,
                                Y = new List<double>() { 8, 9 }
                            },
                        }
                    },
                }
            };

            status.Charts.Add(rangeAreaChart);

            _statusRepository.Update(status);
        }

        public async Task InsertSampleTreeMapChart(long id)
        {
            var status = await _statusRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.KPIs)
                .Include(s => s.Charts)
                .ThenInclude(c => c.Series)
                .ThenInclude(s => s.DataPoints)
                .FirstOrDefaultAsync();

            var treeMapChart = new Chart
            {
                Title = "Tree Map Chart",
                SubTitle = "Some Subtitle",
                Type = ChartType.TreeMap,
                Series = new List<Series>
                {
                    new Series
                    {
                        Name = "Series 1",
                        VizType = VizType.Primary,
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Tag = "Team A",
                                Y = new List<double>() { 30 }
                            },
                            new DataPoint()
                            {
                                Tag = "Team B",
                                Y = new List<double>() { 1 }
                            },
                            new DataPoint()
                            {
                                Tag = "Team C",
                                Y = new List<double>() { 5 }
                            },
                            new DataPoint()
                            {
                                Tag = "Team D",
                                Y = new List<double>() { 9 }
                            },
                        }
                    },
                    new Series
                    {
                        Name = "Series 2",
                        VizType = VizType.Secondary,
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint()
                            {
                                Tag = "Team A",
                                Y = new List<double>() { 7 }
                            },
                            new DataPoint()
                            {
                                Tag = "Team B",
                                Y = new List<double>() { 10 }
                            },
                            new DataPoint()
                            {
                                Tag = "Team C",
                                Y = new List<double>() { 5 }
                            },
                            new DataPoint()
                            {
                                Tag = "Team D",
                                Y = new List<double>() { 6 }
                            },
                        }
                    },
                }
            };

            status.Charts.Add(treeMapChart);

            _statusRepository.Update(status);
        }
    }
}
