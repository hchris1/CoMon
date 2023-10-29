import {Component, Input} from '@angular/core';
import {ChartHelper} from '@shared/helpers/ChartHelper';
import {ChartDto} from '@shared/service-proxies/service-proxies';
import {BaseChartComponent} from '../base-chart/base-chart.component';
import {DarkModeService} from '@app/dark-mode.service';

@Component({
  selector: 'app-radial-bar-chart',
  templateUrl: '../base-chart/base-chart.component.html',
})
export class RadialBarChartComponent extends BaseChartComponent {
  @Input() chart: ChartDto;

  constructor(private _service: DarkModeService) {
    super('radialBar', _service);
  }

  ngOnInit(): void {
    this.apexChart = ChartHelper.createChart('radialBar');
    this.apexLabels = ChartHelper.createLabelsForCircularChart(this.chart);
    this.apexSeries = ChartHelper.createSeriesForCircularChart(this.chart);

    this.apexTitle = {
      text: this.chart.title,
    };
    this.apexSubTitle = {
      text: this.chart.subTitle,
    };
  }
}
