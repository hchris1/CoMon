using Abp.AutoMapper;
using System;
using System.Collections.Generic;

namespace CoMon.Statuses.Dtos
{
    [AutoMapFrom(typeof(Chart))]
    public class ChartDto
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public List<string> Labels { get; set; }
        public Criticality Criticality { get; set; }
        public ChartType Type { get; set; }
        public List<SeriesDto> Series { get; set; }
    }

    [AutoMapFrom(typeof(Series))]
    public class SeriesDto
    {
        public string Name { get; set; }
        public VizType VizType { get; set; }
        public string XUnit { get; set; }
        public string YUnit { get; set; }
        public List<DataPointDto> DataPoints { get; set; }
    }

    [AutoMapFrom(typeof(DataPoint))]
    public class DataPointDto
    {
        public DateTime? Time { get; set; }
        public string Tag { get; set; }
        public double? X { get; set; }
        public List<double> Y { get; set; }
    }
}
