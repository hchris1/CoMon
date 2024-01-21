import {Component, Injector} from '@angular/core';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {AppComponentBase} from '@shared/app-component-base';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  animations: [appModuleAnimation()],
})
export class SettingsComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
