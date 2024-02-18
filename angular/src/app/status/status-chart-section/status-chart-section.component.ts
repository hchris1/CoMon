import {Component, Input} from '@angular/core';
import {NoDataImage} from '@shared/enums/NoDataImage';
import {ChartDto} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-status-chart-section',
  templateUrl: './status-chart-section.component.html',
})
export class StatusChartSectionComponent {
  @Input() charts: ChartDto[];

  image: NoDataImage = NoDataImage.Planet;
}
