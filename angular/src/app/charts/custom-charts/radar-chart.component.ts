import {Component, Input} from '@angular/core';
import {BaseChartComponent} from '../base-chart/base-chart.component';
import {ChartDto} from '@shared/service-proxies/service-proxies';
import {DarkModeService} from '@app/dark-mode.service';
import {ChartHelper} from '@shared/helpers/ChartHelper';

@Component({
  selector: 'app-radar-chart',
  templateUrl: '../base-chart/base-chart.component.html',
})
export class RadarChartComponent extends BaseChartComponent {
  @Input() chart: ChartDto;

  constructor(private _service: DarkModeService) {
    super('radar', _service);
  }

  ngOnInit(): void {
    this.apexLabels = ChartHelper.createLabelsForRadarChart(this.chart);
    this.apexSeries = ChartHelper.createSeriesForRadarChart(this.chart);

    this.apexTitle = {
      text: this.chart.title,
    };
    this.apexSubTitle = {
      text: this.chart.subTitle,
    };
  }
}
