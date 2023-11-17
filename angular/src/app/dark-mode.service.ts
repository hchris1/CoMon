import { Injectable } from '@angular/core';
import { BehaviorSubject, distinctUntilChanged } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DarkModeService {
  isDarkMode: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor() {
    const darkMode = localStorage.getItem('darkMode');
    this.isDarkMode.next(JSON.parse(darkMode));

    this.isDarkMode.pipe(distinctUntilChanged())
      .subscribe((isDarkMode: boolean) => {
        this.setDarkMode(isDarkMode);
      });
  }

  setDarkMode(isDarkMode: boolean) {
    localStorage.setItem('darkMode', isDarkMode.toString());

    if (isDarkMode) {
      document.body.classList.add('dark-mode');
    } else {
      document.body.classList.remove('dark-mode');
    }
  }

  toggleDarkMode() {
    this.isDarkMode.next(!this.isDarkMode.value);
  }
}
