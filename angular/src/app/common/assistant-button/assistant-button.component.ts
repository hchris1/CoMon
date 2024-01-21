import {Component, EventEmitter, Injector, Output} from '@angular/core';
import {AppComponentBase} from '@shared/app-component-base';
import {ConfigurationServiceProxy} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-assistant-button',
  templateUrl: './assistant-button.component.html',
})
export class AssistantButtonComponent extends AppComponentBase {
  @Output() clicked = new EventEmitter<void>();

  isAvailable = false;
  tooltip = 'Assistant is not available';

  constructor(
    injector: Injector,
    configurationService: ConfigurationServiceProxy
  ) {
    super(injector);
    this.tooltip = this.l('Assistant.Unavailable');

    configurationService.getOpenAiKey().subscribe(result => {
      if (result) {
        this.isAvailable = true;
        this.tooltip = undefined;
      }
    });
  }

  onClick(): void {
    this.clicked.emit();
  }
}
