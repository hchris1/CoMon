import {Component, Input} from '@angular/core';
import {NoDataImage} from '@shared/enums/NoDataImage';
import {Criticality} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-status-message-section',
  templateUrl: './status-message-section.component.html',
})
export class StatusMessageSectionComponent {
  @Input() messages: string[];
  @Input() criticality: Criticality;

  image = NoDataImage.Message;
}
