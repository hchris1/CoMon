import {Component, Input} from '@angular/core';
import {NoDataImage} from '@shared/enums/NoDataImage';
import {KPIDto} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-status-kpi-section',
  templateUrl: './status-kpi-section.component.html',
})
export class StatusKpiSectionComponent {
  @Input() kpis: KPIDto[];

  image: NoDataImage = NoDataImage.Compass;
}
