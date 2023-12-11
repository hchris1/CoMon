import { Component, EventEmitter, Input, Output, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { DashboardServiceProxy, DashboardTileDto } from '@shared/service-proxies/service-proxies';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dashboard-tile',
  templateUrl: './dashboard-tile.component.html'
})
export class DashboardTileComponent {
  @Input() dashboardId: number;
  @Input() tile: DashboardTileDto;
  @Input() editMode: boolean = false;
  @Input() isFirst: boolean = false;
  @Input() isLast: boolean = false;

  @Output() tileDeleted = new EventEmitter<DashboardTileDto>();
  @Output() tileMoved = new EventEmitter<DashboardTileDto>();

  confirmDeletionModal: BsModalRef;

  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;

  constructor(
    private _dashboardService: DashboardServiceProxy,
    private _modalService: BsModalService,
    private _router: Router
  ) { }

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
