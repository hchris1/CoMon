import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import {CoMonHubService} from '@app/comon-hub.service';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {
  StatusDto,
  StatusPreviewDto,
  StatusServiceProxy,
} from '@shared/service-proxies/service-proxies';
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

  constructor(
    private _coMonHubService: CoMonHubService,
    private _statusService: StatusServiceProxy
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
}
