import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { GroupPathHelper } from '@shared/helpers/GroupPathHelper';
import { RoutingHelper } from '@shared/helpers/RoutingHelper';
import { AssetDto, GroupPreviewDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-path',
  templateUrl: './path.component.html'
})
export class PathComponent implements OnInit, OnChanges {
  @Input() group: GroupPreviewDto;
  @Input() asset: AssetDto;
  @Input() editMode: boolean = false;
  @Output() rootClicked = new EventEmitter<void>();
  @Output() groupClicked = new EventEmitter<GroupPreviewDto>();
  @Output() assetClicked = new EventEmitter<AssetDto>();
  @Output() pathClicked = new EventEmitter<void>();

  groups: GroupPreviewDto[];

  constructor(
    private _router: Router
  ) { }

  ngOnInit() {
    this.updateGroupHierarchy();
  }

  ngOnChanges() {
    this.updateGroupHierarchy();
  }

  updateGroupHierarchy() {
    this.groups = GroupPathHelper.buildGroupHierarchy(this.group);
  }

  onGroupClick(group: GroupPreviewDto) {
    this._router.navigate(['app', 'overview', group.id], RoutingHelper.buildEditModeQueryParams(this.editMode));
    this.groupClicked.emit(group);
    this.pathClicked.emit();
  }

  onRootClick() {
    this._router.navigate(['app', 'overview'], RoutingHelper.buildEditModeQueryParams(this.editMode));
    this.rootClicked.emit();
    this.pathClicked.emit();
  }

  onAssetClick(asset: AssetDto) {
    this._router.navigate(['app', 'overview', 'assets', asset.id], RoutingHelper.buildEditModeQueryParams(this.editMode));
    this.assetClicked.emit(asset);
    this.pathClicked.emit();
  }
}
