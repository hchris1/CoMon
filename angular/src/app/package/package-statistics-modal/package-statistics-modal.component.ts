import {Component, EventEmitter, Input, Output} from '@angular/core';
import {PackagePreviewDto} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-package-statistics-modal',
  templateUrl: './package-statistics-modal.component.html',
})
export class PackageStatisticsModalComponent {
  @Input() package: PackagePreviewDto;
  @Input() hours: number;
  @Output() onClose = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}
}
