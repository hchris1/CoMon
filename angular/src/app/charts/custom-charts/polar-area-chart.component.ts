import {Component, Input} from '@angular/core';
import {BaseChartComponent} from '../base-chart/base-chart.component';
import {DarkModeService} from '@app/dark-mode.service';
import {ChartDto} from '@shared/service-proxies/service-proxies';
import {ChartHelper} from '@shared/helpers/ChartHelper';

@Component({
  selector: 'app-polar-area-chart',
  templateUrl: '../base-chart/base-chart.component.html',
})
export class PolarAreaChartComponent extends BaseChartComponent {
  @Input() chart: ChartDto;

  constructor(private _service: DarkModeService) {
    super('polarArea', _service);
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
