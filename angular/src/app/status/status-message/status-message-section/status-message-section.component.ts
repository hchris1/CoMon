import {Component, Input} from '@angular/core';
import {NoDataImage} from '@shared/enums/NoDataImage';

@Component({
  selector: 'app-status-message-section',
  templateUrl: './status-message-section.component.html',
})
export class StatusMessageSectionComponent {
  @Input() messages: string[];

  image = NoDataImage.Message;
}
