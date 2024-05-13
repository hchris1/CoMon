import {Component, Input} from '@angular/core';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {Criticality} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-status-message',
  templateUrl: './status-message.component.html',
})
export class StatusMessageComponent {
  @Input() message: string;
  @Input() criticality: Criticality;

  getCardOutlineClass(): string {
    return DynamicStylesHelper.getCardOutlineClass(this.criticality);
  }
}
