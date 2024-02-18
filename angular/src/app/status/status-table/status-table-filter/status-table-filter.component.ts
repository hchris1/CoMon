import {Component, EventEmitter, Input, Output} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {
  AssetPreviewDto,
  GroupPreviewDto,
  Criticality,
  StatusServiceProxy,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-status-table-filter',
  templateUrl: './status-table-filter.component.html',
})
export class StatusTableFilterComponent {
  @Input() showAssetGroupFilter = true;
  @Input() showCriticalityFilter = true;
  @Input() showLatestOnlyFilter = true;
  @Output() filterChanged = new EventEmitter<StatusFilter>();

  statusFilter: StatusFilter = new StatusFilter();

  // Filter options
  options: AssetGroupOption[];
  option: AssetGroupOption;
  criticalities: Criticality[] = [
    undefined,
    Criticality._1,
    Criticality._3,
    Criticality._5,
  ];

  constructor(
    private _router: Router,
    private _route: ActivatedRoute,
    private _statusService: StatusServiceProxy
  ) {
    this.statusFilter = new StatusFilter();

    this.statusFilter.assetId = this._route.snapshot.queryParams.assetId
      ? parseInt(this._route.snapshot.queryParams.assetId, 10)
      : undefined;
    this.statusFilter.groupId = this._route.snapshot.queryParams.groupId
      ? parseInt(this._route.snapshot.queryParams.groupId, 10)
      : undefined;
    this.loadOptions();
  }

  loadOptions() {
    this._statusService.getStatusTableOptions().subscribe(result => {
      this.options = [
        {
          isGroup: true,
          isRoot: true,
          dto: undefined,
        },
      ];
      this.options = this.options
        .concat(
          result.groups
            .sort((a, b) => a.name.localeCompare(b.name))
            .map(group => {
              return {
                isGroup: true,
                isRoot: false,
                dto: group,
              };
            })
        )
        .concat(
          result.assets
            .sort((a, b) => a.name.localeCompare(b.name))
            .map(asset => {
              return {
                isGroup: false,
                isRoot: false,
                dto: asset,
              };
            })
        );

      if (this.statusFilter.assetId) {
        this.option = this.options
          .filter(o => !o.isGroup && !o.isRoot)
          .find(option => option.dto.id === this.statusFilter.assetId);
      } else if (this.statusFilter.groupId) {
        this.option = this.options
          .filter(o => o.isGroup && !o.isRoot)
          .find(option => option.dto.id === this.statusFilter.groupId);
      } else {
        this.option = this.options[0];
      }

      this.filterChanged.emit(this.statusFilter);
    });
  }

  setLatestOnly(latestOnly: boolean) {
    this.statusFilter.latestOnly = latestOnly;
    this.filterChanged.emit(this.statusFilter);
  }

  setCriticality(criticality: Criticality) {
    this.statusFilter.criticality = criticality;
    this.filterChanged.emit(this.statusFilter);
  }

  updateQueryParams() {
    this._router.navigate([], {
      relativeTo: this._route,
      queryParams: {
        assetId: this.statusFilter.assetId,
        groupId: this.statusFilter.groupId,
      },
      queryParamsHandling: 'merge',
    });
  }

  assetGroupChanged(value: AssetGroupOption) {
    this.option = value;

    if (this.option.isRoot) {
      this.statusFilter.assetId = undefined;
      this.statusFilter.groupId = undefined;
    } else if (this.option.isGroup) {
      this.statusFilter.assetId = undefined;
      this.statusFilter.groupId = this.option.dto.id;
    } else {
      this.statusFilter.assetId = this.option.dto.id;
      this.statusFilter.groupId = undefined;
    }

    this.updateQueryParams();
    this.filterChanged.emit(this.statusFilter);
  }
}

class AssetGroupOption {
  isGroup: boolean;
  isRoot: boolean;
  dto: AssetPreviewDto | GroupPreviewDto;
}

export class StatusFilter {
  assetId: number | undefined = undefined;
  groupId: number | undefined = undefined;
  criticality: Criticality | undefined = undefined;
  latestOnly: boolean = true; // Why is this not applied to the filter?
}
