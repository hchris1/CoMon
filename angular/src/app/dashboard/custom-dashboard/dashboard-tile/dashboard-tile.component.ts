import {Component, EventEmitter, Injector, Input, Output} from '@angular/core';
import {Router} from '@angular/router';
import {AppComponentBase} from '@shared/app-component-base';
import {
  DashboardServiceProxy,
  DashboardTileDto,
} from '@shared/service-proxies/service-proxies';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-dashboard-tile',
  templateUrl: './dashboard-tile.component.html',
})
export class DashboardTileComponent extends AppComponentBase {
  @Input() dashboardId: number;
  @Input() tile: DashboardTileDto;
  @Input() editMode: boolean = false;
  @Input() isFirst: boolean = false;
  @Input() isLast: boolean = false;

  @Output() tileDeleted = new EventEmitter<DashboardTileDto>();
  @Output() tileMoved = new EventEmitter<DashboardTileDto>();

  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;

  constructor(
    private _dashboardService: DashboardServiceProxy,
    private _router: Router,
    injector: Injector
  ) {
    super(injector);
  }

  moveUp() {
    this._dashboardService
      .moveTileUp(this.dashboardId, this.tile.id)
      .subscribe(() => {
        this.tileMoved.emit(this.tile);
      });
  }

  moveDown() {
    this._dashboardService
      .moveTileDown(this.dashboardId, this.tile.id)
      .subscribe(() => {
        this.tileMoved.emit(this.tile);
      });
  }

  deleteTileClicked() {
    this.message.confirm(
      this.l('Dashboard.TileDeleteConfirmationMessage'),
      this.l('Dashboard.TileDeleteConfirmationTitle'),
      isConfirmed => {
        if (isConfirmed) {
          this._dashboardService
            .deleteTile(this.dashboardId, this.tile.id)
            .subscribe(() => {
              this.tileDeleted.emit(this.tile);
              this.notify.success(
                this.l('Dashboard.TileDeleteSuccessMessage'),
                this.l('Dashboard.TileDeleteSuccessTitle')
              );
            });
        }
      }
    );
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
