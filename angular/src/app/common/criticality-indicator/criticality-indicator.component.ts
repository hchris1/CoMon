import {Component, Input} from '@angular/core';
import {Criticality} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-criticality-indicator',
  templateUrl: './criticality-indicator.component.html',
})
export class CriticalityIndicatorComponent {
  @Input() criticality: Criticality;

  image: string;

  constructor() {}

  ngOnChanges() {
    this.image = this.getImage();
  }

  getImage() {
    switch (this.criticality) {
      case Criticality._1:
        return 'assets/img/green-circle.svg';
      case Criticality._3:
        return 'assets/img/yellow-circle.svg';
      case Criticality._5:
        return 'assets/img/red-circle.svg';
      default:
        return 'assets/img/grey-circle.svg';
    }
  }
}
