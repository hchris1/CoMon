import {ChangeDetectionStrategy, Component} from '@angular/core';

@Component({
  selector: 'app-loading-animation',
  templateUrl: './loading-animation.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoadingAnimationComponent {
  constructor() {}
}
