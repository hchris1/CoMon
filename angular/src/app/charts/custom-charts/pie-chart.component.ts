import {Component, Input} from '@angular/core';
import {DarkModeService} from '@app/dark-mode.service';
import {ChartHelper} from '@shared/helpers/ChartHelper';
import {ChartDto} from '@shared/service-proxies/service-proxies';
import {BaseChartComponent} from '../base-chart/base-chart.component';

@Component({
  selector: 'app-pie-chart',
  templateUrl: '../base-chart/base-chart.component.html',
})
export class PieChartComponent extends BaseChartComponent {
  @Input() chart: ChartDto;

  constructor(private _service: DarkModeService) {
    super('pie', _service);
  }

  ngOnInit(): void {
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
