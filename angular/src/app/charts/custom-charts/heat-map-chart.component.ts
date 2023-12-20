import {Component, Input} from '@angular/core';
import {BaseChartComponent} from '../base-chart/base-chart.component';
import {DarkModeService} from '@app/dark-mode.service';
import {ChartDto} from '@shared/service-proxies/service-proxies';
import {ChartHelper} from '@shared/helpers/ChartHelper';

@Component({
  selector: 'app-heat-map-chart',
  templateUrl: '../base-chart/base-chart.component.html',
})
export class HeatMapChartComponent extends BaseChartComponent {
  @Input() chart: ChartDto;

  constructor(private _service: DarkModeService) {
    super('heatmap', _service);
  }

  ngOnInit(): void {
    this.apexLabels = ChartHelper.createLabelsForHeatMapChart(this.chart);
    this.apexSeries = ChartHelper.createSeriesForHeatMapChart(this.chart);

    this.apexTitle = {
      text: this.chart.title,
    };
    this.apexSubTitle = {
      text: this.chart.subTitle,
    };
  }
}
