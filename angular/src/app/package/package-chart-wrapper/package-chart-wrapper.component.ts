import {Component, Input} from '@angular/core';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {BehaviorSubject} from 'rxjs';

@Component({
  selector: 'app-package-chart-wrapper',
  templateUrl: './package-chart-wrapper.component.html',
  animations: [appModuleAnimation()],
})
export class PackageChartWrapperComponent {
  @Input() packageId: number;
  @Input() triggerReload: BehaviorSubject<boolean>;
  @Input() numHours: number = 24;
  @Input() triggerRender: BehaviorSubject<boolean>;

  constructor() {}
}
