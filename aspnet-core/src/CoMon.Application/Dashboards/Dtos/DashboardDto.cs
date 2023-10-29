using Abp.AutoMapper;
using System.Collections.Generic;

namespace CoMon.Dashboards.Dtos
{
    [AutoMapFrom(typeof(Dashboard))]
    public class DashboardDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<DashboardTileDto> Tiles { get; set; } = [];
    }
}