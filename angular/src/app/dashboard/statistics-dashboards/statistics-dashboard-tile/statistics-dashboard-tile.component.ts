import {Component, Injector, Input} from '@angular/core';
import {PackageStatisticsModalComponent} from '@app/package/package-statistics-modal/package-statistics-modal.component';
import {AppComponentBase} from '@shared/app-component-base';
import {
  PackagePreviewDto,
  PackageStatisticDto,
} from '@shared/service-proxies/service-proxies';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-statistics-dashboard-tile',
  templateUrl: './statistics-dashboard-tile.component.html',
})
export class StatisticsDashboardTileComponent extends AppComponentBase {
  @Input() statistic: PackageStatisticDto;
  @Input() hours: number;
  @Input() index: number;

  packageStatisticsModalRef: BsModalRef;

  constructor(
    injector: Injector,
    private _modalService: BsModalService
  ) {
    super(injector);
  }

  openPackageStatisticsModal(pack: PackagePreviewDto) {
    this.packageStatisticsModalRef = this._modalService.show(
      PackageStatisticsModalComponent,
      {
        class: 'status-modal',
        initialState: {package: pack, hours: this.hours},
      }
    );
    this.packageStatisticsModalRef.content.closeBtnName = 'Close';

    this.packageStatisticsModalRef.content.onClose.subscribe(() => {
      this.packageStatisticsModalRef.hide();
    });
  }
}
