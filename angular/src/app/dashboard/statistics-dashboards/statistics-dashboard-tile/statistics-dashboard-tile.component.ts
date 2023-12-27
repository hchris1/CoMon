import {Component, Injector, Input, OnInit} from '@angular/core';
import {PackageStatisticsModalComponent} from '@app/package/package-statistics-modal/package-statistics-modal.component';
import {AppComponentBase} from '@shared/app-component-base';
import {
  Criticality,
  PackageHistoryDto,
  PackagePreviewDto,
  PackageStatisticDto,
} from '@shared/service-proxies/service-proxies';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {ProgressbarType} from 'ngx-bootstrap/progressbar';

interface IStack {
  type: ProgressbarType;
  value: number;
  max: number;
}

@Component({
  selector: 'app-statistics-dashboard-tile',
  templateUrl: './statistics-dashboard-tile.component.html',
})
export class StatisticsDashboardTileComponent
  extends AppComponentBase
  implements OnInit
{
  @Input() statistic: PackageStatisticDto;
  @Input() hours: number;
  @Input() index: number;

  timelineStacks: IStack[];
  packageStatisticsModalRef: BsModalRef;

  constructor(
    injector: Injector,
    private _modalService: BsModalService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.timelineStacks = this.buildTimelineStacks(this.statistic.timeline);
  }

  openPackageStatisticsModal(pack: PackagePreviewDto) {
    this.packageStatisticsModalRef = this._modalService.show(
      PackageStatisticsModalComponent,
      {
        class: 'status-modal',
        initialState: {package: pack, hours: this.hours},
      }
    );
    this.packageStatisticsModalRef.content.closeBtnName = 'Close';

    this.packageStatisticsModalRef.content.onClose.subscribe(() => {
      this.packageStatisticsModalRef.hide();
    });
  }

  buildTimelineStacks(timeline: PackageHistoryDto[]): IStack[] {
    const stacks = [];
    for (const entry of timeline.sort((a, b) => a.from.diff(b.from))) {
      switch (entry.criticality) {
        case Criticality._5:
          stacks.push({
            type: 'danger',
            value: entry.percentage,
            max: 1,
          });
          break;
        case Criticality._3:
          stacks.push({
            type: 'warning',
            value: entry.percentage,
            max: 1,
          });
          break;
        case Criticality._1:
          stacks.push({
            type: 'success',
            value: entry.percentage,
            max: 1,
          });
          break;
        default:
          stacks.push({
            type: 'secondary' as ProgressbarType,
            value: entry.percentage,
            max: 1,
          });
          break;
      }
    }
    return stacks;
  }
}
