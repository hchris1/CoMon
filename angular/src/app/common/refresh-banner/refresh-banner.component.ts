import {Component, EventEmitter, Output} from '@angular/core';

@Component({
  selector: 'app-refresh-banner',
  templateUrl: './refresh-banner.component.html',
})
export class RefreshBannerComponent {
  @Output() bannerPressed = new EventEmitter();

  onClick() {
    this.bannerPressed.emit();
  }
}
