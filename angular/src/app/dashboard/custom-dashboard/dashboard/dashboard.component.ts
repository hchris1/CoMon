import {Component, Injector, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {CreateDashboardTileModalComponent} from '@app/edit/create-dashboard-tile-modal/create-dashboard-tile-modal.component';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {AppComponentBase} from '@shared/app-component-base';
import {
  DashboardDto,
  DashboardServiceProxy,
  DashboardTileDto,
  UpdateTilePositionDto,
} from '@shared/service-proxies/service-proxies';
import {NgGridStackOptions, NgGridStackWidget} from 'gridstack/dist/angular';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {
  CONSTRAINTSBYTYPE,
  EDITGRIDOPTIONS,
  STATICGRIDOPTIONS,
} from './dashboard.constants';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  animations: [appModuleAnimation()],
})
export class DashboardComponent extends AppComponentBase {
  id: number;
  dashboard: DashboardDto;

  staticGridOptions: NgGridStackOptions = STATICGRIDOPTIONS;
  editGridOptions: NgGridStackOptions = EDITGRIDOPTIONS;
  gridWidgets: CustomGridStackWidget[];
  constraintsByType = CONSTRAINTSBYTYPE;

  editMode = false;
  editName = false;
  editFormGroup: FormGroup;
  isTrashVisible = false;
  createDashboardTileModal: BsModalRef;

  constructor(
    injector: Injector,
    formBuilder: FormBuilder,
    private _dashboardService: DashboardServiceProxy,
    private _route: ActivatedRoute,
    private _router: Router,
    private _modalService: BsModalService
  ) {
    super(injector);
    this.editFormGroup = formBuilder.group({
      name: ['', [Validators.required]],
    });

    this._route.params.subscribe(params => {
      this.id = params['id'];
      this.loadDashboard();
    });
  }

  loadDashboard() {
    this._dashboardService.get(this.id).subscribe(result => {
      this.dashboard = result;
      this.editFormGroup.controls.name.setValue(this.dashboard.name);

      this.gridWidgets = this.dashboard.tiles.map(tile =>
        this.createWidget(tile)
      );
    });
  }

  createWidget(tile: DashboardTileDto): CustomGridStackWidget {
    return {
      x: tile.x,
      y: tile.y,
      w: tile.width,
      h: tile.height,
      minW: this.constraintsByType[tile.itemType].minW,
      minH: this.constraintsByType[tile.itemType].minH,
      maxW: this.constraintsByType[tile.itemType].maxW,
      maxH: this.constraintsByType[tile.itemType].maxH,
      id: tile.id.toString(),
      tile: tile,
    };
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
        this.dashboard.name = this.editFormGroup.controls.name.value;
      });
  }

  activateEditMode() {
    this.editMode = true;
  }

  deactivateEditMode() {
    // Required so static grid gets changes
    this.gridWidgets = this.dashboard.tiles.map(tile =>
      this.createWidget(tile)
    );
    this.editMode = false;
  }

  routeToDashboards() {
    this._router.navigate(['/app/dashboard']);
  }

  onCreateDashboardTileModal() {
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

  public identify(index: number, w: NgGridStackWidget) {
    return w.id;
  }

  public onGridChange($event: {event: Event; nodes: CustomGridStackWidget[]}) {
    if (!$event.nodes || $event.nodes.length === 0) {
      return;
    }

    const positionDtos = $event.nodes.map(
      node =>
        new UpdateTilePositionDto({
          tileId: node.tile.id,
          x: node.x,
          y: node.y,
          width: node.w,
          height: node.h,
        })
    );

    this._dashboardService
      .updateTilePositions(this.id, positionDtos)
      .subscribe(() => {
        $event.nodes.forEach(node => {
          const tile = this.dashboard.tiles.find(t => t.id === node.tile.id);
          if (tile) {
            Object.assign(tile, {
              x: node.x,
              y: node.y,
              width: node.w,
              height: node.h,
            });
          }
        });
      });
  }

  public onGridRemove($event: {event: Event; nodes: CustomGridStackWidget[]}) {
    if (!$event.nodes || $event.nodes.length === 0) {
      return;
    }

    for (const node of $event.nodes) {
      this._dashboardService.deleteTile(this.id, node.tile.id).subscribe();
    }
  }

  public onDragStart() {
    this.isTrashVisible = true;
  }

  public onDragStop() {
    this.isTrashVisible = false;
  }
}

interface CustomGridStackWidget extends NgGridStackWidget {
  tile: DashboardTileDto;
}
