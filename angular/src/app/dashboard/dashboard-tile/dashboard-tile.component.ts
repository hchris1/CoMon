import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { CoMonHubService } from '@app/comon-hub.service';
import { AssetPreviewDto, AssetServiceProxy, DashboardServiceProxy, DashboardTileDto, GroupDto, GroupPreviewDto, GroupServiceProxy, StatusPreviewDto, StatusServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dashboard-tile',
  templateUrl: './dashboard-tile.component.html'
})
export class DashboardTileComponent implements OnInit {
  @Input() dashboardId: number;
  @Input() tile: DashboardTileDto;
  @Input() editMode: boolean = false;
  @Input() isFirst: boolean = false;
  @Input() isLast: boolean = false;

  @Output() tileDeleted = new EventEmitter<DashboardTileDto>();
  @Output() tileMoved = new EventEmitter<DashboardTileDto>();

  // Tile content
  statusPreview: StatusPreviewDto;

  confirmDeletionModal: BsModalRef;

  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;

  constructor(
    private _assetService: AssetServiceProxy,
    private _dashboardService: DashboardServiceProxy,
    private _groupService: GroupServiceProxy,
    private _statusService: StatusServiceProxy,
    private _modalService: BsModalService,
    private _coMonHubService: CoMonHubService,
    private _changeDetector: ChangeDetectorRef,
    private _router: Router
  ) { }

  ngOnInit() {
    this.loadContent();

    this.statusChangeSubscription = this._coMonHubService.statusUpdate.subscribe((update) => {
      // Only required for package tiles
      if (this.isPackageTile() && update.packageId === this.tile.itemId) {
        this.loadContent();
        this._changeDetector.detectChanges();
      }
    });

    this.connectionEstablishedSubscription = this._coMonHubService.connectionEstablished.subscribe((established) => {
      if (established)
        this.loadContent();
    });
  }

  moveUp() {
    this._dashboardService.moveTileUp(this.dashboardId, this.tile.id)
      .subscribe(() => {
        this.tileMoved.emit(this.tile);
      });
  }

  moveDown() {
    this._dashboardService.moveTileDown(this.dashboardId, this.tile.id)
      .subscribe(() => {
        this.tileMoved.emit(this.tile);
      });
  }

  openDeletionModal(template: TemplateRef<any>) {
    this.confirmDeletionModal = this._modalService.show(template, { class: 'modal-sm' });
  }

  confirmDeletion() {
    this._dashboardService.deleteTile(this.dashboardId, this.tile.id).subscribe(() => {
      this.confirmDeletionModal.hide();
      this.tileDeleted.emit(this.tile);
    });
  }

  cancelDeletion() {
    this.confirmDeletionModal.hide();
  }

  loadContent() {
    if (this.isPackageTile()) {
      this._statusService.getLatestStatusPreview(this.tile.itemId).subscribe((result) => {
        this.statusPreview = result;
      });
    }
  }

  isGroupTile() {
    return this.tile.itemType === 0;
  }

  isAssetTile() {
    return this.tile.itemType === 1;
  }

  isPackageTile() {
    return this.tile.itemType === 2;
  }

  routeToAsset() {
    this._router.navigate(['/app/overview/assets', this.tile.itemId]);
  }

  routeToGroup() {
    this._router.navigate(['/app/overview', this.tile.itemId]);
  }
}
