import { Component, Input } from '@angular/core';
import { BaseChartComponent } from '../base-chart/base-chart.component';
import { DarkModeService } from '@app/dark-mode.service';
import { ChartDto } from '@shared/service-proxies/service-proxies';
import { ChartHelper } from '@shared/helpers/ChartHelper';

@Component({
  selector: 'app-range-area-chart',
  templateUrl: '../base-chart/base-chart.component.html'
})
export class RangeAreaChartComponent extends BaseChartComponent {
  @Input() chart: ChartDto;

  isTimeBased: boolean;

  constructor(
    private _service: DarkModeService
  ) {
    super('rangeArea', _service);
  }

  ngOnInit(): void {
    this.isTimeBased = ChartHelper.isTimeBasedChart(this.chart);

    this.apexSeries = this.isTimeBased
      ? ChartHelper.createSeriesForTimeBasedRangedChart(this.chart)
      : ChartHelper.createSeriesForXYRangedChart(this.chart);
    this.buildXAxis();
    this.buildYAxis();

    this.apexTitle = {
      text: this.chart.title
    }
    this.apexSubTitle = {
      text: this.chart.subTitle
    }
  }

  ngAfterViewInit(): void {
    super.ngAfterViewInit();
  }


  buildXAxis() {
    if (this.isTimeBased)
      this.apexXAxis = ChartHelper.createXAxisForTimeBasedChart();
    else {
      this.apexXAxis = ChartHelper.createXAxisForNumericChart(this.chart.series.length > 0 ? this.chart.series[0].xUnit : '');
    }
  }

  buildYAxis() {
    this.apexYAxis = ChartHelper.createYAxisForNumericChart(this.chart.series.length > 0 ? this.chart.series[0].yUnit : '');
  }
}
