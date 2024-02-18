import {
  Component,
  ElementRef,
  Input,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import {CoMonHubService} from '@app/comon-hub.service';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {
  StatusServiceProxy,
  StatusPreviewDtoPagedResultDto,
  StatusPreviewDto,
  StatusDto,
} from '@shared/service-proxies/service-proxies';
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

  @ViewChild('scrollContainer') scrollContainer: ElementRef;

  statusPreviews: StatusPreviewDtoPagedResultDto;
  status: StatusDto;
  hasMoreStatuses = true;
  loadingMoreStatuses = false;
  isLoadingStatus = false;

  assistantModalRef: BsModalRef;

  maxLoadCount = 20;

  // Status change
  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;

  statusFilter: StatusFilter;

  constructor(
    private _statusService: StatusServiceProxy,
    private _coMonHubService: CoMonHubService,
    private _modalService: BsModalService
  ) {}

  ngOnInit(): void {
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
    this.loadStatuses();
  }

  ngOnDestroy(): void {
    this.statusChangeSubscription.unsubscribe();
    this.connectionEstablishedSubscription.unsubscribe();
  }

  onStatusClicked(status: StatusPreviewDto) {
    this.status = undefined;
    this.isLoadingStatus = true;
    this._statusService.get(status.id).subscribe(result => {
      this.status = result;
      this.isLoadingStatus = false;
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
        this.statusPreviews.items
          .filter(
            statusPreview => statusPreview.package.id === update.packageId
          )
          .forEach(statusPreview => {
            statusPreview.isLatest = false;
          });

        if (
          this.statusFilter.assetId &&
          this.statusFilter.assetId !== update.assetId
        )
          return;
        if (
          this.statusFilter.groupId &&
          update.groupIds &&
          !update.groupIds.includes(this.statusFilter.groupId)
        )
          return;

        if (this.status && this.status.package.id === update.packageId) {
          this.status.isLatest = false;
        }

        this._statusService.getPreview(update.id).subscribe(result => {
          this.statusPreviews.items.unshift(result);
        });
      });
  }

  loadStatuses() {
    this.status = undefined;
    this.statusPreviews = undefined;

    this._statusService
      .getStatusTable(
        0,
        this.maxLoadCount,
        this.statusFilter?.assetId,
        this.statusFilter?.groupId,
        this.packageId,
        this.statusFilter?.criticality,
        this.statusFilter?.latestOnly
      )
      .subscribe(result => {
        this.statusPreviews = result;
        this.hasMoreStatuses = result.totalCount > result.items.length;
      });
  }

  loadMoreStatuses() {
    this.loadingMoreStatuses = true;
    this._statusService
      .getStatusTable(
        this.statusPreviews.items.length,
        this.maxLoadCount,
        this.statusFilter?.assetId,
        this.statusFilter?.groupId,
        this.packageId,
        this.statusFilter?.criticality,
        this.statusFilter?.latestOnly
      )
      .subscribe(result => {
        this.statusPreviews.items = this.statusPreviews.items.concat(
          result.items
        );
        this.hasMoreStatuses =
          result.totalCount > this.statusPreviews.items.length;

        this.loadingMoreStatuses = false;
      });
  }

  onScroll(event): void {
    if (
      event.target.offsetHeight + event.target.scrollTop >=
      event.target.scrollHeight - 10
    ) {
      if (this.hasMoreStatuses && !this.loadingMoreStatuses) {
        this.loadMoreStatuses();
      }
    }
  }
}
