import {Component, Input} from '@angular/core';
import {
  Criticality,
  PackageHistoryDto,
} from '@shared/service-proxies/service-proxies';
import {ProgressbarType} from 'ngx-bootstrap/progressbar';

interface IStack {
  type: ProgressbarType;
  value: number;
  max: number;
}

@Component({
  selector: 'app-statistics-dashboard-timeline',
  templateUrl: './statistics-dashboard-timeline.component.html',
})
export class StatisticsDashboardTimelineComponent {
  @Input() timeline: PackageHistoryDto[];

  timelineStacks: IStack[];

  ngOnChanges(): void {
    this.timelineStacks = this.buildTimelineStacks(this.timeline);
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
