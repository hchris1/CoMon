import {ChartDto, VizType} from '@shared/service-proxies/service-proxies';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexNonAxisChartSeries,
  ApexXAxis,
  ApexYAxis,
  ChartType,
} from 'ng-apexcharts';

export class ChartHelper {
  static createLabelsForHeatMapChart(chart: ChartDto): string[] {
    return chart.labels;
  }

  static createSeriesForHeatMapChart(chart: ChartDto): ApexAxisChartSeries {
    const apexSeries: ApexAxisChartSeries = [];
    for (const series of chart.series) {
      apexSeries.push({
        name: series.name,
        data: series.dataPoints.length > 0 ? series.dataPoints[0].y : [],
        color: ChartHelper.getColorForVizType(series.vizType),
      });
    }
    return apexSeries;
  }

  static createLabelsForRadarChart(chart: ChartDto): string[] {
    return chart.labels;
  }

  static createSeriesForRadarChart(chart: ChartDto): ApexAxisChartSeries {
    return this.createSeriesForHeatMapChart(chart);
  }

  static createChart(type: ChartType): ApexChart {
    return {
      height: '300',
      width: '100%',
      type: type,
      fontFamily: 'inherit',
      zoom: {
        enabled: false,
      },
      toolbar: {
        show: false,
      },
      background: 'none',
    };
  }

  static createXAxisForTimeBasedChart(): ApexXAxis {
    return {
      type: 'datetime',
    };
  }

  static createSeriesForTreeMapChart(chart: ChartDto): ApexAxisChartSeries {
    const apexSeries: ApexAxisChartSeries = [];
    for (const series of chart.series) {
      apexSeries.push({
        name: series.name,
        data: series.dataPoints.map(x => {
          return {
            x: x.tag,
            y: x.y[0],
          };
        }),
        color: ChartHelper.getColorForVizType(series.vizType),
      });
    }
    return apexSeries;
  }

  static createXAxisForNumericChart(xUnit: string): ApexXAxis {
    return {
      type: 'numeric',
      title: {
        text: xUnit || '',
      },
    };
  }

  static createYAxisForNumericChart(yUnit: string): ApexYAxis {
    return {
      title: {
        text: yUnit || '',
      },
    };
  }

  static createSeriesForTimeBasedChart(chart: ChartDto): ApexAxisChartSeries {
    const apexSeries: ApexAxisChartSeries = [];
    for (const series of chart.series) {
      apexSeries.push({
        name: series.name,
        data: series.dataPoints
          .sort((a, b) => a.time.valueOf() - b.time.valueOf())
          .map(x => {
            return [x.time.valueOf(), x.y[0]];
          }),
        color: ChartHelper.getColorForVizType(series.vizType),
      });
    }
    return apexSeries;
  }

  static createSeriesForXYChart(chart: ChartDto): ApexAxisChartSeries {
    const apexSeries: ApexAxisChartSeries = [];
    for (const series of chart.series) {
      apexSeries.push({
        name: series.name,
        data: series.dataPoints
          .sort((a, b) => a.x - b.x)
          .map(x => {
            return [x.x, x.y[0]];
          }),
        color: ChartHelper.getColorForVizType(series.vizType),
      });
    }
    return apexSeries;
  }

  static createSeriesForTimeBasedRangedChart(
    chart: ChartDto
  ): ApexAxisChartSeries {
    const apexSeries: ApexAxisChartSeries = [];
    for (const series of chart.series) {
      apexSeries.push({
        name: series.name,
        data: series.dataPoints
          .sort((a, b) => a.time.valueOf() - b.time.valueOf())
          .map(x => {
            return {
              x: x.time.valueOf(),
              y: x.y,
            };
          }),
        color: ChartHelper.getColorForVizType(series.vizType),
      });
    }
    return apexSeries;
  }

  static createSeriesForXYRangedChart(chart: ChartDto): ApexAxisChartSeries {
    const apexSeries: ApexAxisChartSeries = [];
    for (const series of chart.series) {
      apexSeries.push({
        name: series.name,
        data: series.dataPoints
          .sort((a, b) => a.x - b.x)
          .map(x => {
            return {
              x: x.x,
              y: x.y,
            };
          }),
        color: ChartHelper.getColorForVizType(series.vizType),
      });
    }
    return apexSeries;
  }

  static isTimeBasedChart(chart: ChartDto): boolean {
    if (chart.series.length > 0) {
      if (chart.series[0].dataPoints.length > 0) {
        return chart.series[0].dataPoints[0].time !== undefined;
      }
    }
    return false;
  }

  static createSeriesForCircularChart(chart: ChartDto): ApexNonAxisChartSeries {
    const apexSeries: ApexNonAxisChartSeries = [];
    for (const series of chart.series) {
      if (series.dataPoints.length > 0) {
        apexSeries.push(...series.dataPoints[0].y);
      }
    }
    return apexSeries;
  }

  static createLabelsForCircularChart(chart: ChartDto): string[] {
    return chart.labels;
  }

  static getColorForVizType(vizType: VizType): string {
    switch (vizType) {
      case VizType._0:
        return '#007bff';
      case VizType._1:
        return '#6c757d';
      case VizType._2:
        return '#28a745';
      case VizType._3:
        return '#dc3545';
      case VizType._4:
        return '#ffc107';
      case VizType._5:
        return '#17a2b8';
      default:
        return '#007bff';
    }
  }
}
