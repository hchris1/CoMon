import {Component, Input} from '@angular/core';
import {KPIDto} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-status-kpi',
  templateUrl: './status-kpi.component.html',
  styleUrls: ['./status-kpi.component.css'],
})
export class StatusKpiComponent {
  @Input() kpi: KPIDto;
}
