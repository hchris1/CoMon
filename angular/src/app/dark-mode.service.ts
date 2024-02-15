import {Injectable} from '@angular/core';
import {BehaviorSubject, distinctUntilChanged} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DarkModeService {
  isDarkMode: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor() {
    const darkMode = JSON.parse(localStorage.getItem('darkMode'));

    if (darkMode) {
      this.isDarkMode.next(darkMode);
    }

    this.isDarkMode
      .pipe(distinctUntilChanged())
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
