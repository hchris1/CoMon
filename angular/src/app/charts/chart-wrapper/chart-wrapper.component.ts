import {ChangeDetectorRef, Component, Input} from '@angular/core';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {
  ChartDto,
  ChartType,
  StatusServiceProxy,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-chart-wrapper',
  templateUrl: './chart-wrapper.component.html',
  animations: [appModuleAnimation()],
})
export class ChartWrapperComponent {
  @Input() chart: ChartDto;
  isDarkMode: boolean;

  constructor(
    private _changeDetection: ChangeDetectorRef,
    private _statusService: StatusServiceProxy
  ) {}

  isLineChart(chart: ChartDto): boolean {
    return chart.type === ChartType._1;
  }

  isAreaChart(chart: ChartDto): boolean {
    return chart.type === ChartType._2;
  }

  isBarChart(chart: ChartDto): boolean {
    return chart.type === ChartType._3;
  }

  isPieChart(chart: ChartDto): boolean {
    return chart.type === ChartType._4;
  }

  isDonutChart(chart: ChartDto): boolean {
    return chart.type === ChartType._5;
  }

  isRadialBarChart(chart: ChartDto): boolean {
    return chart.type === ChartType._6;
  }

  isScatterChart(chart: ChartDto): boolean {
    return chart.type === ChartType._7;
  }

  isHeatMapChart(chart: ChartDto): boolean {
    return chart.type === ChartType._8;
  }

  isRadarChart(chart: ChartDto): boolean {
    return chart.type === ChartType._9;
  }

  isPolarAreaChart(chart: ChartDto): boolean {
    return chart.type === ChartType._10;
  }

  isRangeAreaChart(chart: ChartDto): boolean {
    return chart.type === ChartType._11;
  }

  isTreeMapChart(chart: ChartDto): boolean {
    return chart.type === ChartType._12;
  }
}
