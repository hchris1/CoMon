import {ChangeDetectorRef, Component, Injector} from '@angular/core';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {AppComponentBase} from '@shared/app-component-base';
import {
  PackageServiceProxy,
  PackageStatisticDto,
} from '@shared/service-proxies/service-proxies';
import {ProgressbarType} from 'ngx-bootstrap/progressbar';
import * as moment from 'moment';

interface IStack {
  type: ProgressbarType;
  value: number;
  max: number;
}

@Component({
  selector: 'app-statistics-dashboard',
  templateUrl: './statistics-dashboard.component.html',
  animations: [appModuleAnimation()],
})
export class StatisticsDashboardComponent extends AppComponentBase {
  statistics: PackageStatisticDto[];
  statisticWithStacks: [PackageStatisticDto, IStack[]][];
  refreshIntervalSeconds = 5 * 60;
  nextRefresh: moment.Moment;
  refreshInterval: NodeJS.Timeout;

  times = [
    {
      hours: 1,
      name: 'Last Hour',
    },
    {
      hours: 24,
      name: 'Last 24 Hours',
    },
    {
      hours: 168,
      name: 'Last Week',
    },
    {
      hours: 720,
      name: 'Last Month',
    },
  ];
  selectedTime = this.times[1];

  constructor(
    injector: Injector,
    private _changeDetector: ChangeDetectorRef,
    private _packageService: PackageServiceProxy
  ) {
    super(injector);

    this.loadStatistics();
    this.createRefreshInterval();
  }

  createRefreshInterval() {
    this.nextRefresh = moment().add(this.refreshIntervalSeconds, 'seconds');
    this.refreshInterval = setInterval(() => {
      this.loadStatistics();
      this.nextRefresh = moment().add(this.refreshIntervalSeconds, 'seconds');
    }, this.refreshIntervalSeconds * 1000);
  }

  onTimeChange() {
    clearInterval(this.refreshInterval);
    this.loadStatistics();
    this.createRefreshInterval();
  }

  loadStatistics() {
    this._packageService
      .getStatistics(this.selectedTime.hours)
      .subscribe(result => {
        this.statistics = this.sortStatistics(result);
        this.statisticWithStacks = this.buildStatisticWithStacks(
          this.statistics
        );
        this._changeDetector.detectChanges();
      });
  }

  buildStatisticWithStacks(
    statistics: PackageStatisticDto[]
  ): [PackageStatisticDto, IStack[]][] {
    const statisticWithStacks = [];
    for (const statistic of statistics) {
      const stack: IStack[] = [];
      stack.push(...this.buildStack(statistic));
      statisticWithStacks.push([statistic, stack]);
    }
    return statisticWithStacks;
  }

  buildStack(statistics: PackageStatisticDto): IStack[] {
    const stack: IStack[] = [];
    stack.push({
      type: 'danger',
      value: statistics.alertPercent,
      max: 1,
    });
    stack.push({
      type: 'warning',
      value: statistics.warningPercent,
      max: 1,
    });
    stack.push({
      type: 'success',
      value: statistics.healthyPercent,
      max: 1,
    });
    stack.push({
      type: 'secondary' as ProgressbarType,
      value: statistics.unknownPercent,
      max: 1,
    });
    return stack;
  }

  sortStatistics(statistics: PackageStatisticDto[]): PackageStatisticDto[] {
    // Sort statistics by biggest alertPercent, then warningPercent, then healthyPercent
    return statistics.sort((a, b) => {
      if (a.alertPercent > b.alertPercent) {
        return -1;
      } else if (a.alertPercent < b.alertPercent) {
        return 1;
      } else if (a.warningPercent > b.warningPercent) {
        return -1;
      } else if (a.warningPercent < b.warningPercent) {
        return 1;
      } else if (a.healthyPercent > b.healthyPercent) {
        return -1;
      } else if (a.healthyPercent < b.healthyPercent) {
        return 1;
      } else {
        return 0;
      }
    });
  }
}
