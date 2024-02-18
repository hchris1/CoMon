import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import {CoMonHubService, StatusUpdateDto} from '@app/comon-hub.service';
import {
  StatusServiceProxy,
  StatusPreviewDtoPagedResultDto,
  StatusPreviewDto,
  StatusDto,
} from '@shared/service-proxies/service-proxies';
import {Subscription} from 'rxjs';
import {StatusFilter} from './status-table-filter/status-table-filter.component';

@Component({
  selector: 'app-status-table',
  templateUrl: './status-table.component.html',
})
export class StatusTableComponent implements OnInit, OnDestroy {
  @ViewChild('scrollContainer') scrollContainer: ElementRef;

  status: StatusDto;
  statusFilter: StatusFilter;
  statusPreviews: StatusPreviewDtoPagedResultDto;

  hasMoreStatuses = true;
  isLoadingMoreStatusPreviews = false;
  isLoadingStatus = false;

  batchSize = 20;

  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;

  constructor(
    private _statusService: StatusServiceProxy,
    private _coMonHubService: CoMonHubService
  ) {}

  ngOnInit(): void {
    this.subscribeToStatusChanges();

    this.connectionEstablishedSubscription =
      this._coMonHubService.connectionEstablished.subscribe(established => {
        if (established) this.loadStatuses();
      });
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

  subscribeToStatusChanges() {
    this.statusChangeSubscription =
      this._coMonHubService.statusUpdate.subscribe(update => {
        this.updateStatusPreviews(update);
        if (!this.isRelevantUpdate(update)) return;
        this.markCurrentStatusAsNotLatest(update);
        this.prependNewStatusPreview(update);
      });
  }

  updateStatusPreviews(update: StatusUpdateDto) {
    this.statusPreviews.items.forEach(statusPreview => {
      if (statusPreview.package.id === update.packageId) {
        if (this.statusFilter.latestOnly && statusPreview.isLatest) {
          this.statusPreviews.items = this.statusPreviews.items.filter(
            x => x.id !== statusPreview.id
          );
        } else {
          statusPreview.isLatest = false;
        }
      }
    });
  }

  isRelevantUpdate(update) {
    if (
      this.statusFilter.assetId &&
      this.statusFilter.assetId !== update.assetId
    ) {
      return false;
    }
    if (
      this.statusFilter.groupId &&
      update.groupIds &&
      !update.groupIds.includes(this.statusFilter.groupId)
    ) {
      return false;
    }
    return true;
  }

  markCurrentStatusAsNotLatest(update) {
    if (this.status && this.status.package.id === update.packageId) {
      this.status.isLatest = false;
    }
  }

  prependNewStatusPreview(update) {
    this._statusService.getPreview(update.id).subscribe(result => {
      this.statusPreviews.items.unshift(result);
    });
  }

  loadStatuses(loadMore = false) {
    if (!loadMore) {
      this.status = undefined;
      this.statusPreviews = undefined;
    } else {
      this.isLoadingMoreStatusPreviews = true;
    }

    this._statusService
      .getStatusTable(
        loadMore ? this.statusPreviews.items.length : 0,
        this.batchSize,
        this.statusFilter?.assetId,
        this.statusFilter?.groupId,
        undefined,
        this.statusFilter?.criticality,
        this.statusFilter?.latestOnly
      )
      .subscribe(result => {
        if (loadMore) {
          this.statusPreviews.items = this.statusPreviews.items.concat(
            result.items
          );
          this.isLoadingMoreStatusPreviews = false;
        } else {
          this.statusPreviews = result;
        }

        this.hasMoreStatuses =
          result.totalCount > this.statusPreviews.items.length;
      });
  }

  onScroll(event): void {
    if (
      event.target.offsetHeight + event.target.scrollTop >=
      event.target.scrollHeight - 10
    ) {
      if (this.hasMoreStatuses && !this.isLoadingMoreStatusPreviews) {
        this.loadStatuses(true);
      }
    }
  }
}
