import {Component, Injector, Input, OnInit, ViewChild} from '@angular/core';
import {DarkModeService} from '@app/dark-mode.service';
import {AppComponentBase} from '@shared/app-component-base';
import {
  PackageServiceProxy,
  PackageStatusCountDto,
} from '@shared/service-proxies/service-proxies';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexGrid,
  ApexPlotOptions,
  ApexTheme,
  ApexTitleSubtitle,
  ApexXAxis,
  ApexYAxis,
  ChartComponent,
} from 'ng-apexcharts';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  plotOptions: ApexPlotOptions;
  grid: ApexGrid;
  theme: ApexTheme;
  title: ApexTitleSubtitle;
  subTitle: ApexTitleSubtitle;
};

@Component({
  selector: 'app-package-bar-chart',
  templateUrl: './package-bar-chart.component.html',
})
export class PackageBarChartComponent
  extends AppComponentBase
  implements OnInit
{
  @Input() packageId: number;
  @Input() numHours = 24;
  @Input() useHourBuckets = true;
  @Input() useChanges = false;

  @ViewChild('chartObj') chart: ChartComponent;

  data: PackageStatusCountDto[];
  series: ApexAxisChartSeries;

  chartOptions: Partial<ChartOptions>;

  constructor(
    private _packageService: PackageServiceProxy,
    private _darkModeService: DarkModeService,
    injector: Injector
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    if (!this.useChanges)
      this._packageService
        .getPackageStatusUpdateBuckets(
          this.packageId,
          this.numHours,
          this.useHourBuckets
        )
        .subscribe((data: PackageStatusCountDto[]) => {
          this.data = data;
          this.transformData();
          this.createChartOptions();
        });
    else
      this._packageService
        .getPackageStatusChangeBuckets(
          this.packageId,
          this.numHours,
          this.useHourBuckets
        )
        .subscribe((data: PackageStatusCountDto[]) => {
          this.data = data;
          this.transformData();
          this.createChartOptions();
        });
  }

  transformData() {
    this.series = [
      {
        name: 'Healthy',
        data: this.data.map(e => {
          return {
            x: e.date,
            y: e.healthyCount,
          };
        }),
        color: '#28a745',
      },
      {
        name: 'Warning',
        data: this.data.map(e => {
          return {
            x: e.date,
            y: e.warningCount,
          };
        }),
        color: '#ffc107',
      },
      {
        name: 'Alert',
        data: this.data.map(e => {
          return {
            x: e.date,
            y: e.alertCount,
          };
        }),
        color: '#dc3545',
      },
    ];
  }

  createChartOptions() {
    this.chartOptions = {
      chart: {
        type: 'bar',
        height: 250,
        toolbar: {
          show: true,
        },
        stacked: true,
        background: 'none',
        fontFamily: 'inherit',
      },
      series: this.series,
      xaxis: {
        type: 'datetime',
        labels: {
          datetimeUTC: false,
        },
      },
      theme: {
        mode: this._darkModeService.isDarkMode.value ? 'dark' : 'light',
      },
      title: {
        text: this.useChanges
          ? this.l('Dashboard.ChangesChartTitle')
          : this.l('Dashboard.UpdatesChartTitle'),
      },
      subTitle: {
        text: this.useChanges
          ? this.l('Dashboard.ChangesChartDescription')
          : this.l('Dashboard.UpdatesChartDescription'),
      },
    };
  }
}
