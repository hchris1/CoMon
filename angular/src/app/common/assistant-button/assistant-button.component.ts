import {Component, EventEmitter, Injector, Output} from '@angular/core';
import {AppComponentBase} from '@shared/app-component-base';

@Component({
  selector: 'app-assistant-button',
  templateUrl: './assistant-button.component.html',
})
export class AssistantButtonComponent extends AppComponentBase {
  @Output() clicked = new EventEmitter<void>();

  isAvailable = false;
  tooltip = 'Assistant is not available';

  constructor(injector: Injector) {
    super(injector);
  }

  onClick(): void {
    this.clicked.emit();
  }
}
