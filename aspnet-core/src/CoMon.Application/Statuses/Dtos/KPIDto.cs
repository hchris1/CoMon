using Abp.AutoMapper;

namespace CoMon.Statuses.Dtos
{
    [AutoMapFrom(typeof(KPI))]
    public class KPIDto
    {
        public string Name { get; set; }
        public double? Value { get; set; }
        public string Unit { get; set; }
        public double? ThirtyDayAverage { get; set; }
        public double? ThirtyDayMax { get; set; }
        public double? ThirtyDayMin { get; set; }
    }
}
