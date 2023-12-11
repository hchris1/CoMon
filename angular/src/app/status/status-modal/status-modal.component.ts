import { ChangeDetectorRef, Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { CoMonHubService } from '@app/comon-hub.service';
import { DynamicStylesHelper } from '@shared/helpers/DynamicStylesHelper';
import { StatusDto, StatusPreviewDto, StatusServiceProxy } from '@shared/service-proxies/service-proxies';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-status-modal',
  templateUrl: './status-modal.component.html'
})
export class StatusModalComponent implements OnInit, OnDestroy {
  @Input() statusId: number;
  @Output() onClose = new EventEmitter();

  status: StatusDto;
  reloadHistory: EventEmitter<StatusPreviewDto> = new EventEmitter<StatusPreviewDto>();
  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;

  constructor(
    private _coMonHubService: CoMonHubService,
    private _statusService: StatusServiceProxy,
    private _changeDetector: ChangeDetectorRef,
    private _router: Router
  ) { }

  ngOnInit(): void {
    this.loadStatus();
    this.statusChangeSubscription = this._coMonHubService.statusUpdate.subscribe((update) => {
      if (this.status.package.id === update.packageId) {
        this.loadStatus();
        this.reloadHistory.emit(this.status);
      }
    });

    this.connectionEstablishedSubscription = this._coMonHubService.connectionEstablished.subscribe((established) => {
      if (established)
        this.loadStatus();
    });
  }

  ngOnDestroy(): void {
    this.statusChangeSubscription.unsubscribe();
    this.connectionEstablishedSubscription.unsubscribe();
  }

  loadStatus() {
    this._statusService.get(this.statusId).subscribe((result) => {
      this.status = result;
      this._changeDetector.detectChanges();
    });
  }

  closeClicked() {
    this.onClose.emit();
  }

  getBackgroundStyle() {
    return DynamicStylesHelper.getHistoricBackgroundClass(this.status.isLatest);
  }

  getEmoji() {
    return DynamicStylesHelper.getEmoji(this.status.criticality);
  }

  switchModal(statusPreview: StatusPreviewDto) {
    this.statusId = statusPreview.id;
    this.loadStatus();
    this.reloadHistory.emit(statusPreview);
  }

  routeToPackage() {
    this._router.navigate(['app', 'overview', 'packages', this.status.package.id]);
    this.closeClicked();
  }
}
