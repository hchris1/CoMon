import { Component, Input, ViewChild } from '@angular/core';
import { StatusModalComponent } from '@app/status/status-modal/status-modal.component';
import { ChartHelper } from '@shared/helpers/ChartHelper';
import { Criticality, PackageServiceProxy, StatusPreviewDto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { ApexChart, ApexAxisChartSeries, ApexXAxis, ApexPlotOptions, ApexGrid, ApexYAxis, ChartComponent } from 'ng-apexcharts';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  plotOptions: ApexPlotOptions;
  grid: ApexGrid;
};
@Component({
  selector: 'app-package-timeline-chart',
  templateUrl: './package-timeline-chart.component.html'
})


export class PackageTimelineChartComponent {

  @Input() packageId: number;
  @Input() numOfHours = 12;
  @ViewChild('chartObj') chart: ChartComponent;
  statusPreviews: StatusPreviewDto[];

  data: any[];
  statusModalRef: BsModalRef;

  chartOptions: ChartOptions;

  minDate: moment.Moment;
  maxDate: moment.Moment;

  constructor(
    private _packageService: PackageServiceProxy,
    private _modalService: BsModalService
  ) { }

  ngOnInit(): void {
    this._packageService.getPackageStatusHistory(this.packageId, this.numOfHours).subscribe((statusPreviews: StatusPreviewDto[]) => {
      this.statusPreviews = statusPreviews.sort((a, b) => a.time.valueOf() - b.time.valueOf());
      this.setBoundaryDates();
      this.transformData();
      console.log(this.data)
      this.createChartOptions();
    });
  }

  setBoundaryDates() {
    this.minDate = moment().utc().subtract(this.numOfHours, 'hour');
    this.maxDate = moment().utc();
  }

  transformData() {
    this.data = this.statusPreviews
      .map((status, index) => {
        const nextStatus = this.statusPreviews[index + 1];

        return {
          criticality: status.criticality,
          x: (status.criticality === Criticality._1 ? 'Healthy' : status.criticality === Criticality._3 ? 'Warning' : 'Error'),
          y: [status.time.valueOf(), nextStatus ? nextStatus.time.valueOf() : this.maxDate.valueOf()],
          from: status.time.toISOString(),
          to: nextStatus ? nextStatus.time.toISOString() : this.maxDate.toISOString(),
          color: ChartHelper.getColorForCriticality(status.criticality)
        };
      });
  }

  createChartOptions() {

    this.chartOptions = {
      series: [
        {
          name: 'Healthy',
          data: this.data.filter(x => x.criticality === Criticality._1),
          color: '#28a745'
        },
        {
          name: 'Warning',
          data: this.data.filter(x => x.criticality === Criticality._3),
          color: '#ffc107'
        },
        {
          name: 'Error',
          data: this.data.filter(x => x.criticality === Criticality._5),
          color: '#dc3545'
        }],
      chart: {
        height: 100,
        type: "rangeBar",
        toolbar: {
          show: true
        },
        sparkline: {
          enabled: true
        },
        animations: {
          enabled: false
        },
        events: {
          click: (event, chartContext, config) => {
            if (config.dataPointIndex >= 0)
              this.openStatusModal(this.statusPreviews.filter(x => x.criticality === (config.seriesIndex === 0 ? Criticality._1 : config.seriesIndex === 1 ? Criticality._3 : Criticality._5))
              [parseInt(config.dataPointIndex, 10)]);
          }
        },
        zoom: {
          enabled: true
        }
      },
      grid: {
        show: false
      },
      plotOptions: {
        bar: {
          horizontal: true,
          barHeight: '100%',
          rangeBarGroupRows: true
        }
      },
      xaxis: {
        type: "datetime",
        labels: {
          datetimeUTC: false,
        }
      },
      yaxis: {
        show: false,
        min: this.minDate.valueOf(),
        max: this.maxDate.add(1, 'hour').valueOf(),
      }
    };
  }

  openStatusModal(statusPreview: StatusPreviewDto) {
    this.statusModalRef = this._modalService.show(StatusModalComponent,
      {
        class: 'status-modal',
        initialState: { statusId: statusPreview.id },
      }
    );
    this.statusModalRef.content.closeBtnName = 'Close';

    this.statusModalRef.content.onClose.subscribe(() => {
      this.statusModalRef.hide();
    });
  }
}
