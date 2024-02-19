import {ChangeDetectorRef, Component, Input, OnInit} from '@angular/core';
import {DarkModeService} from '@app/dark-mode.service';
import {Gradients} from '@shared/enums/Gradients';
import {Observable, interval, map, takeWhile, tap} from 'rxjs';

@Component({
  selector: 'app-statistics-tile',
  templateUrl: './statistics-tile.component.html',
})
export class StatisticsTileComponent implements OnInit {
  @Input() title: string;
  @Input() value: number;

  currentValue: number = 0;

  gradientColors: string[];
  gradientAngle = Math.floor(Math.random() * 360);

  constructor(
    private _darkModeService: DarkModeService,
    private _changeDetector: ChangeDetectorRef
  ) {
    this._darkModeService.isDarkMode.subscribe(isDarkMode => {
      this.gradientColors = isDarkMode
        ? Gradients.kpiDarkModeGradient
        : Gradients.kpiBrightModeGradient;
    });
  }

  ngOnInit(): void {
    // Do an animation by counting up to the value
    this.animateCount(0, this.value).subscribe(value => {
      this.currentValue = Math.round(value);
      this._changeDetector.detectChanges();
    });
  }

  animateCount(
    start: number,
    end: number,
    duration: number = 10000
  ): Observable<number> {
    const stepTime = 20; // Time between each step in ms
    const totalSteps = duration / stepTime;
    let currentStep = 0;

    // Define the easeOutQuint function
    const easeOutQuint = (x: number) => 1 - Math.pow(1 - x, 5);

    return interval(stepTime).pipe(
      map(() => {
        currentStep++;
        const stepRatio = currentStep / totalSteps;
        const easedStep = easeOutQuint(stepRatio); // Apply the easeOutQuint function
        return start + (end - start) * easedStep;
      }),
      takeWhile(value => {
        // Ensure the animation stops at the right time for both counting up and down
        return end > start
          ? value <= end && currentStep <= totalSteps
          : value >= end && currentStep <= totalSteps;
      }),
      tap({
        complete: () => console.log('Animation completed!'),
      })
    );
  }
}
