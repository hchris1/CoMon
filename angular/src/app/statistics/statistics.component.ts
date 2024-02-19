import {Component} from '@angular/core';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {
  StatisticsDto,
  StatisticsServiceProxy,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  animations: [appModuleAnimation()],
})
export class StatisticsComponent {
  statistics: StatisticsDto;

  constructor(private _statisticsService: StatisticsServiceProxy) {
    this._statisticsService.get().subscribe(statistics => {
      this.statistics = statistics;
    });
  }
}
