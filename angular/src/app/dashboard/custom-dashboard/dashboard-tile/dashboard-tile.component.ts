import {Component, Injector, Input} from '@angular/core';
import {Router} from '@angular/router';
import {AppComponentBase} from '@shared/app-component-base';
import {DashboardTileDto} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-dashboard-tile',
  templateUrl: './dashboard-tile.component.html',
})
export class DashboardTileComponent extends AppComponentBase {
  @Input() dashboardId: number;
  @Input() tile: DashboardTileDto;
  @Input() editMode: boolean = false;

  constructor(
    private _router: Router,
    injector: Injector
  ) {
    super(injector);
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

  isMarkdownTile() {
    return this.tile.itemType === 3;
  }

  routeToAsset() {
    this._router.navigate(['/app/overview/assets', this.tile.itemId]);
  }

  routeToGroup() {
    this._router.navigate(['/app/overview', this.tile.itemId]);
  }
}
