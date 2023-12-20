import {Component, Input} from '@angular/core';
import {DarkModeService} from '@app/dark-mode.service';
import {ChartHelper} from '@shared/helpers/ChartHelper';
import {ChartDto} from '@shared/service-proxies/service-proxies';
import {BaseChartComponent} from '../base-chart/base-chart.component';

@Component({
  selector: 'app-area-chart',
  templateUrl: '../base-chart/base-chart.component.html',
})
export class AreaChartComponent extends BaseChartComponent {
  @Input() chart: ChartDto;

  isTimeBased: boolean;

  constructor(private _service: DarkModeService) {
    super('area', _service);
  }

  ngOnInit(): void {
    this.isTimeBased = ChartHelper.isTimeBasedChart(this.chart);
    this.apexSeries = this.isTimeBased
      ? ChartHelper.createSeriesForTimeBasedChart(this.chart)
      : ChartHelper.createSeriesForXYChart(this.chart);
    this.buildXAxis();
    this.buildYAxis();

    this.apexTitle = {
      text: this.chart.title,
    };
    this.apexSubTitle = {
      text: this.chart.subTitle,
    };
  }

  buildXAxis() {
    if (this.isTimeBased)
      this.apexXAxis = ChartHelper.createXAxisForTimeBasedChart();
    else {
      this.apexXAxis = ChartHelper.createXAxisForNumericChart(
        this.chart.series.length > 0 ? this.chart.series[0].xUnit : ''
      );
    }
  }

  buildYAxis() {
    this.apexYAxis = ChartHelper.createYAxisForNumericChart(
      this.chart.series.length > 0 ? this.chart.series[0].yUnit : ''
    );
  }
}
