import {
  AfterViewInit,
  Component,
  Inject,
  OnDestroy,
  ViewChild,
} from '@angular/core';
import {DarkModeService} from '@app/dark-mode.service';
import {ChartHelper} from '@shared/helpers/ChartHelper';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexNonAxisChartSeries,
  ApexTheme,
  ApexTitleSubtitle,
  ApexXAxis,
  ApexYAxis,
  ChartComponent,
  ChartType,
} from 'ng-apexcharts';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-base-chart',
  templateUrl: './base-chart.component.html',
})
export class BaseChartComponent implements AfterViewInit, OnDestroy {
  @ViewChild('chart') chartRef: ChartComponent;
  chartType: ChartType;
  apexSeries: ApexAxisChartSeries | ApexNonAxisChartSeries;
  apexChart: ApexChart;
  apexXAxis: ApexXAxis;
  apexYAxis: ApexYAxis;
  apexTitle: ApexTitleSubtitle;
  apexSubTitle: ApexTitleSubtitle;
  apexTheme: ApexTheme;
  apexLabels: string[];

  isDarkModeSubscription: Subscription;

  constructor(
    @Inject(String) private _chartType: ChartType,
    private _darkModeService: DarkModeService
  ) {
    this.apexChart = ChartHelper.createChart(_chartType);
    this.apexTheme = {
      mode: _darkModeService.isDarkMode.value ? 'dark' : 'light',
    };
  }
  ngOnDestroy(): void {
    this.isDarkModeSubscription.unsubscribe();
  }

  ngAfterViewInit() {
    this.isDarkModeSubscription = this._darkModeService.isDarkMode.subscribe(
      (isDarkMode: boolean) => {
        const theme = isDarkMode ? 'dark' : 'light';
        if (this.apexTheme.mode === theme) {
          return;
        }

        this.apexTheme = {
          mode: theme,
        };

        this.chartRef.updateOptions(
          {
            theme: this.apexTheme,
          },
          false,
          false
        );
      }
    );
  }
}
