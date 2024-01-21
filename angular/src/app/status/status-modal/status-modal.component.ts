import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import {AssistantModalComponent} from '@app/common/assistant-modal/assistant-modal.component';
import {CoMonHubService} from '@app/comon-hub.service';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {
  StatusDto,
  StatusPreviewDto,
  StatusServiceProxy,
} from '@shared/service-proxies/service-proxies';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {Subscription, skip} from 'rxjs';

@Component({
  selector: 'app-status-modal',
  templateUrl: './status-modal.component.html',
})
export class StatusModalComponent implements OnInit, OnDestroy {
  @Input() statusId: number;
  @Output() onClose = new EventEmitter();

  status: StatusDto;
  reloadHistory: EventEmitter<boolean> = new EventEmitter<boolean>();
  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;
  assistantModalRef: BsModalRef;

  constructor(
    private _coMonHubService: CoMonHubService,
    private _statusService: StatusServiceProxy,
    private _modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.loadStatus();
    this.statusChangeSubscription =
      this._coMonHubService.statusUpdate.subscribe(update => {
        if (this.status.package.id === update.packageId) {
          this.loadStatus();
          this.reloadHistory.emit(true);
        }
      });

    this.connectionEstablishedSubscription =
      this._coMonHubService.connectionEstablished
        .pipe(skip(1))
        .subscribe(established => {
          if (established) {
            this.loadStatus();
            this.reloadHistory.emit(true);
          }
        });
  }

  ngOnDestroy(): void {
    this.statusChangeSubscription.unsubscribe();
    this.connectionEstablishedSubscription.unsubscribe();
  }

  loadStatus() {
    this._statusService.get(this.statusId).subscribe(result => {
      this.status = result;
    });
  }

  closeClicked() {
    this.onClose.emit();
  }

  getBackgroundStyle() {
    return DynamicStylesHelper.getHistoricBackgroundClass(this.status.isLatest);
  }

  switchModal(statusPreview: StatusPreviewDto) {
    this.statusId = statusPreview.id;
    this.status = undefined;
    this.loadStatus();
  }

  openAssistant() {
    this.assistantModalRef = this._modalService.show(AssistantModalComponent, {
      initialState: {
        statusId: this.statusId,
      },
      class: 'modal-lg',
    });
    this.assistantModalRef.content.closeBtnName = 'Close';

    this.assistantModalRef.content.onClose.subscribe(() => {
      this.assistantModalRef.hide();
    });
  }
}
