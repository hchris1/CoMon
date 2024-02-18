import {Component, Input, OnInit} from '@angular/core';
import {DarkModeService} from '@app/dark-mode.service';
import {KPIDto} from '@shared/service-proxies/service-proxies';
import {Gradients} from '@shared/enums/Gradients';

@Component({
  selector: 'app-status-kpi',
  templateUrl: './status-kpi.component.html',
  styleUrls: ['./status-kpi.component.css'],
})
export class StatusKpiComponent implements OnInit {
  @Input() kpi: KPIDto;

  isStatisticAvailable = false;
  isHigherThanAverage: boolean | undefined = undefined;
  isLowerThanAverage: boolean | undefined = undefined;
  percentage: number | undefined = undefined;

  gradientColors: string[];

  constructor(private _darkModeService: DarkModeService) {
    this._darkModeService.isDarkMode.subscribe(isDarkMode => {
      this.gradientColors = isDarkMode
        ? Gradients.kpiDarkModeGradient
        : Gradients.kpiBrightModeGradient;
    });
  }

  ngOnInit(): void {
    this.isStatisticAvailable =
      this.kpi && this.kpi.value !== null && this.kpi.thirtyDayAverage !== null;

    if (!this.isStatisticAvailable) return;

    this.isHigherThanAverage = this.kpi.value > this.kpi.thirtyDayAverage;
    this.isLowerThanAverage = this.kpi.value < this.kpi.thirtyDayAverage;

    this.percentage = Math.round(
      (Math.abs(this.kpi.value - this.kpi.thirtyDayAverage) /
        this.kpi.thirtyDayAverage) *
        100
    );
  }
}
