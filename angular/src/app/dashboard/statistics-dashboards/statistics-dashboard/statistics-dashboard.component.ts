import {ChangeDetectorRef, Component, Injector} from '@angular/core';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {AppComponentBase} from '@shared/app-component-base';
import {
  PackageHistoryDto,
  PackageServiceProxy,
  PackageStatisticDto,
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import {Router} from '@angular/router';
import {
  REFRESHINTERVALSECONDS,
  TIMESPANOPTIONS,
} from './statistics-dashboard.constants';

@Component({
  selector: 'app-statistics-dashboard',
  templateUrl: './statistics-dashboard.component.html',
  animations: [appModuleAnimation()],
})
export class StatisticsDashboardComponent extends AppComponentBase {
  statistics: PackageStatisticDto[];
  nextRefresh: moment.Moment;
  refreshInterval: NodeJS.Timeout;
  timeline: PackageHistoryDto[];

  timespanOptions = TIMESPANOPTIONS;
  selectedTime = this.timespanOptions[1];

  constructor(
    injector: Injector,
    private _changeDetector: ChangeDetectorRef,
    private _packageService: PackageServiceProxy,
    private _router: Router
  ) {
    super(injector);

    this.loadStatistics();
    this.createRefreshInterval();
  }

  createRefreshInterval() {
    this.nextRefresh = moment().add(REFRESHINTERVALSECONDS, 'seconds');
    this.refreshInterval = setInterval(() => {
      this.loadStatistics();
      this.nextRefresh = moment().add(REFRESHINTERVALSECONDS, 'seconds');
    }, REFRESHINTERVALSECONDS * 1000);
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
        this._changeDetector.detectChanges();
      });
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

  routeToDashboards() {
    this._router.navigate(['/app/dashboard']);
  }
}
