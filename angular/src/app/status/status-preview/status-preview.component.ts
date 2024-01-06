import {
  Component,
  EventEmitter,
  Injector,
  Input,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {
  PackageServiceProxy,
  PackageStatisticDto,
  StatusPreviewDto,
} from '@shared/service-proxies/service-proxies';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {StatusModalComponent} from '../status-modal/status-modal.component';
import {EditPackageModalComponent} from '@app/edit/edit-package-modal/edit-package-modal.component';
import {Clipboard} from '@angular/cdk/clipboard';
import {AppComponentBase} from '@shared/app-component-base';

@Component({
  selector: 'app-status-preview',
  templateUrl: './status-preview.component.html',
  encapsulation: ViewEncapsulation.None,
})
export class StatusPreviewComponent extends AppComponentBase {
  @Input() statusPreview: StatusPreviewDto;
  @Input() showPath: boolean = true;
  @Input() editMode: boolean = false;
  @Input() showDate: boolean = true;
  @Input() showTimeline: boolean = false;
  @Output() packageDeleted = new EventEmitter();
  @Output() packageEdited = new EventEmitter();

  statusModalRef: BsModalRef;
  editModalRef: BsModalRef;
  statistic: PackageStatisticDto;
  statisticInterval: NodeJS.Timeout;

  constructor(
    private _modalService: BsModalService,
    private _packageService: PackageServiceProxy,
    private _clipboard: Clipboard,
    injector: Injector
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (!this.showTimeline) return;

    this.loadStatistic();

    this.statisticInterval = setInterval(
      () => {
        this.loadStatistic();
      },
      5 * 60 * 1000
    );
  }

  ngOnDestroy(): void {
    clearInterval(this.statisticInterval);
  }

  loadStatistic() {
    this._packageService
      .getStatistic(this.statusPreview.package.id, 24)
      .subscribe(result => {
        this.statistic = result;
      });
  }

  getBackgroundStyle() {
    return DynamicStylesHelper.getHistoricBackgroundClass(
      this.statusPreview.isLatest
    );
  }

  getCardOutlineClass() {
    return DynamicStylesHelper.getCardOutlineClass(
      this.statusPreview.criticality
    );
  }

  openStatusModal() {
    if (this.editMode) return;

    this.statusModalRef = this._modalService.show(StatusModalComponent, {
      class: 'status-modal',
      initialState: {statusId: this.statusPreview.id},
    });
    this.statusModalRef.content.closeBtnName = 'Close';

    this.statusModalRef.content.onClose.subscribe(() => {
      this.statusModalRef.hide();
    });
  }

  deletePackageClicked() {
    this.message.confirm(
      this.l('Package.DeleteConfirmationMessage'),
      this.l('Package.DeleteConfirmationTitle'),
      isConfirmed => {
        if (isConfirmed) {
          this._packageService
            .delete(this.statusPreview.package.id)
            .subscribe(() => {
              this.packageDeleted.emit();
              this.notify.success(
                this.l('Package.DeleteSuccessMessage'),
                this.statusPreview.package.name
              );
            });
        }
      }
    );
  }

  editClicked() {
    this.editModalRef = this._modalService.show(EditPackageModalComponent, {
      initialState: {packageId: this.statusPreview.package.id},
    });
    this.editModalRef.content.closeBtnName = 'Close';

    this.editModalRef.content.onEdited.subscribe(() => {
      this.packageEdited.emit();
      this.editModalRef.hide();
    });

    this.editModalRef.content.onClose.subscribe(() => {
      this.editModalRef.hide();
    });
  }

  copyClicked() {
    this._clipboard.copy(this.statusPreview.package.guid);
  }
}
