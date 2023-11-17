import { Component } from '@angular/core';
import { DarkModeService } from '@app/dark-mode.service';

@Component({
  selector: 'header-mode-toggle',
  templateUrl: './header-mode-toggle.component.html'
})
export class HeaderModeToggleComponent {
  constructor(private darkModeService: DarkModeService) { }

  toggleDarkMode() {
    this.darkModeService.toggleDarkMode();
  }

  get isDarkMode() {
    return this.darkModeService.isDarkMode.value;
  }
}
