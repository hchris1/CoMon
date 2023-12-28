import {ChangeDetectionStrategy, Component} from '@angular/core';

@Component({
  selector: 'app-loading-animation',
  templateUrl: './loading-animation.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoadingAnimationComponent {
  images = [
    'assets/img/loading-1.svg',
    'assets/img/loading-2.svg',
    'assets/img/loading-3.svg',
  ];

  constructor() {}

  getRandomImage() {
    return this.images[Math.floor(Math.random() * this.images.length)];
  }
}
