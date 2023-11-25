import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { DarkModeService } from '@app/dark-mode.service';
import { PackageServiceProxy, PackageStatusCountDto } from '@shared/service-proxies/service-proxies';
import { ApexAxisChartSeries, ApexChart, ApexGrid, ApexPlotOptions, ApexTheme, ApexXAxis, ApexYAxis, ChartComponent } from 'ng-apexcharts';
import { BehaviorSubject } from 'rxjs';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  plotOptions: ApexPlotOptions;
  grid: ApexGrid;
  theme: ApexTheme;
};

@Component({
  selector: 'app-package-bar-chart',
  templateUrl: './package-bar-chart.component.html'
})
export class PackageBarChartComponent implements OnInit {
  @Input() packageId: number;
  @Input() numHours = 24;
  @Input() useHourBuckets = true;
  @Input() useChanges = false;
  @Input() triggerRender: BehaviorSubject<boolean>;
  @Input() triggerReload: BehaviorSubject<boolean>;

  @ViewChild('chartObj') chart: ChartComponent;

  data: PackageStatusCountDto[];
  series: ApexAxisChartSeries;

  chartOptions: Partial<ChartOptions>;

  constructor(
    private _packageService: PackageServiceProxy,
    private _darkModeService: DarkModeService
  ) {
    this._darkModeService.isDarkMode.subscribe(() => {
      this.createChartOptions();
    });
  }

  ngOnInit(): void {
    this.loadData();

    if (this.triggerRender) {
      this.triggerRender.subscribe((val) => {
        if (this.chart && val)
          this.chart.updateOptions(this.chartOptions);
      })
    }

    if (this.triggerReload) {
      this.triggerReload.subscribe((val) => {
        if (val)
          this.loadData();
      })
    }
  }

  loadData() {
    if (!this.useChanges)
      this._packageService.getPackageStatusUpdateBuckets(this.packageId, this.numHours, this.useHourBuckets).subscribe((data: PackageStatusCountDto[]) => {
        this.data = data;
        this.transformData();
        this.createChartOptions();
      });
    else
      this._packageService.getPackageStatusChangeBuckets(this.packageId, this.numHours, this.useHourBuckets).subscribe((data: PackageStatusCountDto[]) => {
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
            x: e.date.valueOf(),
            y: e.healthyCount
          }
        }),
        color: '#28a745'
      },
      {
        name: 'Warning',
        data: this.data.map(e => {
          return {
            x: e.date.valueOf(),
            y: e.warningCount
          }
        }),
        color: '#ffc107'
      },
      {
        name: 'Alert',
        data: this.data.map(e => {
          return {
            x: e.date.valueOf(),
            y: e.alertCount
          }
        }),
        color: '#dc3545'
      }
    ];
  }

  createChartOptions() {
    this.chartOptions = {
      chart: {
        type: 'bar',
        height: 250,
        toolbar: {
          show: true
        },
        stacked: true,
        background: 'none'
      },
      series: this.series,
      xaxis: {
        type: 'datetime'
      },
      theme: {
        mode: this._darkModeService.isDarkMode.value ? 'dark' : 'light'
      },
    }
  }
}
