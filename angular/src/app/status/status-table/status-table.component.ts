import {
  ChangeDetectorRef,
  Component,
  Input,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {CoMonHubService} from '@app/comon-hub.service';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {
  StatusServiceProxy,
  StatusPreviewDtoPagedResultDto,
  StatusPreviewDto,
  StatusDto,
} from '@shared/service-proxies/service-proxies';
import {PageChangedEvent} from 'ngx-bootstrap/pagination';
import {BehaviorSubject, Subscription} from 'rxjs';
import {StatusFilter} from './status-table-filter/status-table-filter.component';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {AssistantModalComponent} from '@app/common/assistant-modal/assistant-modal.component';

@Component({
  selector: 'app-status-table',
  templateUrl: './status-table.component.html',
})
export class StatusTableComponent implements OnInit, OnDestroy {
  @Input() showAssetGroupFilter = true;
  @Input() showCriticalityFilter = true;
  @Input() showLatestOnlyFilter = true;
  @Input() packageId = undefined;
  @Input() showRefreshBanner = true;
  @Input() triggerReload: BehaviorSubject<boolean>;

  statusPreviews: StatusPreviewDtoPagedResultDto;
  status: StatusDto;

  assistantModalRef: BsModalRef;

  // Pagination
  maxResultCount = 10;
  skipCount = 0;
  currentPage = 1;

  // Query params
  assetId: number;
  groupId: number;

  // Status change
  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;
  statusChanged = false;

  statusFilter: StatusFilter;

  constructor(
    private _statusService: StatusServiceProxy,
    private _coMonHubService: CoMonHubService,
    private _changeDetector: ChangeDetectorRef,
    private _route: ActivatedRoute,
    private _modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.assetId = parseInt(this._route.snapshot.queryParams['assetId'], 10);
    this.groupId = parseInt(this._route.snapshot.queryParams['groupId'], 10);

    this.subscribeToStatusChanges();

    this.connectionEstablishedSubscription =
      this._coMonHubService.connectionEstablished.subscribe(established => {
        if (established) this.loadStatuses();
      });

    if (this.triggerReload) {
      this.triggerReload.subscribe(val => {
        if (val) {
          this.loadStatuses();
        }
      });
    }
  }

  filterChanged(filter: StatusFilter) {
    this.statusFilter = filter;
    this.resetPagination();
    this.loadStatuses();
  }

  ngOnDestroy(): void {
    this.statusChangeSubscription.unsubscribe();
    this.connectionEstablishedSubscription.unsubscribe();
  }

  onStatusClicked(status: StatusPreviewDto) {
    this._statusService.get(status.id).subscribe(result => {
      this.status = result;
    });
  }

  getBackgroundStyle() {
    return DynamicStylesHelper.getHistoricBackgroundClass(this.status.isLatest);
  }

  openAssistant() {
    this.assistantModalRef = this._modalService.show(AssistantModalComponent, {
      initialState: {
        statusId: this.status.id,
      },
      class: 'modal-lg',
    });
    this.assistantModalRef.content.closeBtnName = 'Close';

    this.assistantModalRef.content.onClose.subscribe(() => {
      this.assistantModalRef.hide();
    });
  }

  subscribeToStatusChanges() {
    this.statusChangeSubscription =
      this._coMonHubService.statusUpdate.subscribe(update => {
        this.statusChanged = true;
        this.statusPreviews.items
          .filter(
            statusPreview => statusPreview.package.id === update.packageId
          )
          .forEach(statusPreview => {
            statusPreview.isLatest = false;
          });
        this._changeDetector.detectChanges();
      });
  }

  loadStatuses() {
    this.statusPreviews = undefined;

    this._statusService
      .getStatusTable(
        this.skipCount,
        this.maxResultCount,
        this.statusFilter.assetId,
        this.statusFilter.groupId,
        this.packageId,
        this.statusFilter.criticality,
        this.statusFilter.latestOnly
      )
      .subscribe(result => {
        this.statusPreviews = result;
        this.statusChanged = false;
      });
  }

  changePage(event: PageChangedEvent): void {
    if (event.page === this.currentPage) return;

    this.skipCount = (event.page - 1) * event.itemsPerPage;
    this.maxResultCount = event.itemsPerPage;
    this.loadStatuses();
  }

  resetPagination() {
    this.skipCount = 0;
    this.currentPage = 1;
  }
}
