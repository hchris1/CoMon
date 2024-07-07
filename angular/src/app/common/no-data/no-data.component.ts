import {Component, Input} from '@angular/core';
import {NoDataImage} from '@shared/enums/NoDataImage';

@Component({
  selector: 'app-no-data',
  templateUrl: './no-data.component.html',
})
export class NoDataComponent {
  @Input() message: string = 'NoDataAvailable';
  @Input() width: number = 7;
  @Input() image: NoDataImage = NoDataImage.PersonWithStatus;
}
