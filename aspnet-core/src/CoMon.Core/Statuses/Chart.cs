using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Statuses
{
    [Table("CoMonCharts")]
    public class Chart : Entity<long>
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public List<string> Labels { get; set; }
        public ChartType Type { get; set; }
        public List<Series> Series { get; set; } = [];

        [ForeignKey(nameof(Status))]
        public long StatusId { get; set; }
        public Status Status { get; set; }
    }

    [Table("CoMonSeries")]
    public class Series : Entity<long>
    {
        public string Name { get; set; }
        public VizType VizType { get; set; }
        public string XUnit { get; set; }
        public string YUnit { get; set; }
        public List<DataPoint> DataPoints { get; set; } = [];

        [ForeignKey(nameof(Chart))]
        public long ChartId { get; set; }
        public Chart Chart { get; set; }
    }

    [Table("CoMonDataPoints")]
    public class DataPoint : Entity<long>
    {
        public DateTime? Time { get; set; }
        public string Tag { get; set; } // Needed for TreeMaps
        public double? X { get; set; }
        public List<double> Y { get; set; } = []; // Support for multiple values needed for RangeArea

        [ForeignKey(nameof(Series))]
        public long SeriesId { get; set; }
        public Series Series { get; set; }
    }

    public enum VizType
    {
        Primary = 0,
        Secondary = 1,
        Success = 2,
        Danger = 3,
        Warning = 4,
        Info = 5,
    }

    public enum ChartType
    {
        Line = 1,
        Area = 2,
        Bar = 3,
        Pie = 4,
        Donut = 5,
        RadialBar = 6,
        Scatter = 7,
        HeatMap = 8,
        Radar = 9,
        PolarArea = 10,
        RangeArea = 11,
        TreeMap = 12
    }
}
