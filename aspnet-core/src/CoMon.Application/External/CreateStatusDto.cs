using Abp.AutoMapper;
using CoMon.Statuses;
using System;
using System.Collections.Generic;

namespace CoMon.External
{
    [AutoMapTo(typeof(Status))]
    public class CreateStatusDto
    {
        public Criticality Criticality { get; set; }
        public List<string> Messages { get; set; }
        public List<CreateKpiDto> KPIs { get; set; }
        public List<CreateChartDto> Charts { get; set; }
    }

    [AutoMapTo(typeof(KPI))]
    public class CreateKpiDto
    {
        public string Name { get; set; }
        public double? Value { get; set; }
        public string Unit { get; set; }
    }

    [AutoMapTo(typeof(Chart))]
    public class CreateChartDto
    {
        public string Title { get; set; }
        public string SubTitle { get; set; } = null;
        public List<string> Labels { get; set; } = [];
        public ChartType Type { get; set; }
        public List<CreateSeriesDto> Series { get; set; }
    }

    [AutoMapTo(typeof(Series))]
    public class CreateSeriesDto
    {
        public string Name { get; set; }
        public VizType VizType { get; set; }
        public string XUnit { get; set; } = null;
        public string YUnit { get; set; } = null;
        public List<CreateDataPointDto> DataPoints { get; set; } = [];
    }

    [AutoMapTo(typeof(DataPoint))]
    public class CreateDataPointDto
    {
        public DateTime? Time { get; set; } = null;
        public string Tag { get; set; } = null;
        public double? X { get; set; } = null;
        public List<double> Y { get; set; } = [];
    }
}