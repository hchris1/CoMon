import {
  Component,
  EventEmitter,
  Input,
  Output,
  TemplateRef,
  ViewEncapsulation,
} from '@angular/core';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {
  PackageServiceProxy,
  StatusPreviewDto,
} from '@shared/service-proxies/service-proxies';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {StatusModalComponent} from '../status-modal/status-modal.component';
import {EditPackageModalComponent} from '@app/edit/edit-package-modal/edit-package-modal.component';
import {Clipboard} from '@angular/cdk/clipboard';

@Component({
  selector: 'app-status-preview',
  templateUrl: './status-preview.component.html',
  encapsulation: ViewEncapsulation.None,
})
export class StatusPreviewComponent {
  @Input() statusPreview: StatusPreviewDto;
  @Input() showPath: boolean = true;
  @Input() editMode: boolean = false;
  @Input() showDate: boolean = true;
  @Output() packageDeleted = new EventEmitter();
  @Output() packageEdited = new EventEmitter();

  statusModalRef: BsModalRef;
  confirmDeletionModal: BsModalRef;
  editModalRef: BsModalRef;

  constructor(
    private _modalService: BsModalService,
    private _packageService: PackageServiceProxy,
    private _clipboard: Clipboard
  ) {}

  getEmoji() {
    return DynamicStylesHelper.getEmoji(this.statusPreview.criticality);
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

  /* eslint-disable @typescript-eslint/no-explicit-any */
  deleteClicked(template: TemplateRef<any>) {
    this.confirmDeletionModal = this._modalService.show(template, {
      class: 'modal-sm',
    });
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

  confirmDeletion() {
    this._packageService.delete(this.statusPreview.package.id).subscribe(() => {
      this.confirmDeletionModal.hide();
      this.packageDeleted.emit();
    });
  }

  declineDeletion() {
    this.confirmDeletionModal.hide();
  }

  copyClicked() {
    this._clipboard.copy(this.statusPreview.package.guid);
  }
}
