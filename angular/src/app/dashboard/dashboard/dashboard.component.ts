import {Component} from '@angular/core';
import {
  DashboardDto,
  DashboardServiceProxy,
  DashboardTileDto,
} from '@shared/service-proxies/service-proxies';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {RoutingHelper} from '@shared/helpers/RoutingHelper';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {CreateDashboardTileModalComponent} from '@app/edit/create-dashboard-tile-modal/create-dashboard-tile-modal.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  animations: [appModuleAnimation()],
})
export class DashboardComponent {
  editMode = false;
  editName = false;
  dashboard: DashboardDto;
  editFormGroup: FormGroup;
  id: number;

  createDashboardTileModal: BsModalRef;

  constructor(
    formBuilder: FormBuilder,
    private _dashboardService: DashboardServiceProxy,
    private _route: ActivatedRoute,
    private _router: Router,
    private _modalService: BsModalService
  ) {
    this.editFormGroup = formBuilder.group({
      name: ['', [Validators.required]],
    });

    _route.queryParams.subscribe(params => {
      this.editMode = params['editMode'] === 'true';
    });

    this._route.params.subscribe(params => {
      this.id = params['id'];
      this.loadDashboard();
    });
  }

  loadDashboard() {
    this._dashboardService.get(this.id).subscribe(result => {
      this.dashboard = result;
      this.dashboard.tiles = this.dashboard.tiles.sort(
        (a, b) => a.sortIndex - b.sortIndex
      );
      this.editFormGroup.controls.name.setValue(this.dashboard.name);
    });
  }

  editNameClicked() {
    this.editFormGroup.controls.name.setValue(this.dashboard.name);
    this.editName = true;
  }

  saveNameClicked() {
    this.editName = false;
    this._dashboardService
      .updateName(this.dashboard.id, this.editFormGroup.controls.name.value)
      .subscribe(() => {
        this.loadDashboard();
      });
  }

  activateEditMode() {
    this._router.navigate([], {
      relativeTo: this._route,
      queryParams: RoutingHelper.buildEditModeQueryParams(true),
      queryParamsHandling: 'merge',
    });
    this.editMode = true;
  }

  deactivateEditMode() {
    this._router.navigate([], {
      relativeTo: this._route,
      queryParams: RoutingHelper.buildEditModeQueryParams(false),
      queryParamsHandling: 'merge',
    });
    this.editMode = false;
  }

  isFirst(tile: DashboardTileDto) {
    return (
      tile.sortIndex === Math.min(...this.dashboard.tiles.map(x => x.sortIndex))
    );
  }

  isLast(tile: DashboardTileDto) {
    return (
      tile.sortIndex === Math.max(...this.dashboard.tiles.map(x => x.sortIndex))
    );
  }

  routeToDashboards() {
    this._router.navigate(['/app/dashboard']);
  }

  onCreateDashboardModal() {
    this.createDashboardTileModal = this._modalService.show(
      CreateDashboardTileModalComponent,
      {
        class: 'modal-lg',
        initialState: {
          dashboardId: this.dashboard.id,
        },
      }
    );
    this.createDashboardTileModal.content.closeBtnName = 'Close';

    this.createDashboardTileModal.content.onClose.subscribe(() => {
      this.createDashboardTileModal.hide();
    });

    this.createDashboardTileModal.content.onCreated.subscribe(() => {
      this.loadDashboard();
    });
  }
}
