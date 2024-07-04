import {Component, Input, OnChanges, OnInit} from '@angular/core';
import {Criticality} from '@shared/service-proxies/service-proxies';
import {PackageType} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-criticality-indicator',
  templateUrl: './criticality-indicator.component.html',
})
export class CriticalityIndicatorComponent implements OnInit, OnChanges{
  @Input() criticality: Criticality;
  @Input() packageType: PackageType;

  colorClass: string;
  iconClass: string;

  constructor() {}

  ngOnInit() {
    this.iconClass = this.getIconClass();
    this.colorClass = this.getColorClass();
  }

  ngOnChanges() {
    this.colorClass = this.getColorClass();
  }

  private getIconClass(): string {
    switch (this.packageType) {
      // Ping
      case PackageType._0:
        return 'fa-solid fa-network-wired';
      // HTTP
      case PackageType._1:
        return 'fa-solid fa-server';
      // RTSP
      case PackageType._2:
        return 'fa-solid fa-video';
      // External
      case PackageType._10:
        return 'fa-solid fa-cloud';

      default:
        return 'fa-solid fa-circle';
    }
  }

  private getColorClass(): string {
    switch (this.criticality) {
      case Criticality._1:
        return 'text-success';
      case Criticality._3:
        return 'text-warning';
      case Criticality._5:
        return 'text-danger';
      default:
        return 'text-secondary';
    }
  }
}
