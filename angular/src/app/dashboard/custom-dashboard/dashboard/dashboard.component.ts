import {Component, Injector} from '@angular/core';
import {
  DashboardDto,
  DashboardServiceProxy,
  DashboardTileDto,
  DashboardTileType,
  UpdateTilePositionDto,
} from '@shared/service-proxies/service-proxies';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {CreateDashboardTileModalComponent} from '@app/edit/create-dashboard-tile-modal/create-dashboard-tile-modal.component';
import {NgGridStackOptions, NgGridStackWidget} from 'gridstack/dist/angular';
import {AppComponentBase} from '@shared/app-component-base';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  animations: [appModuleAnimation()],
})
export class DashboardComponent extends AppComponentBase {
  editMode = false;
  editName = false;
  dashboard: DashboardDto;
  editFormGroup: FormGroup;
  id: number;
  createDashboardTileModal: BsModalRef;
  gridOptions: NgGridStackOptions;
  gridWidgets: CustomGridStackWidget[];

  isTrashVisible = false;

  constraintsByTileType = {
    [DashboardTileType._0]: {minW: 2, minH: 2, maxW: 6, maxH: 3},
    [DashboardTileType._1]: {minW: 2, minH: 2, maxW: 6, maxH: 8},
    [DashboardTileType._2]: {minW: 2, minH: 2, maxW: 6, maxH: 4},
    [DashboardTileType._3]: {minW: 1, minH: 2, maxW: 12, maxH: 8},
  };

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

    this.gridOptions = {
      margin: 5,
      float: true,
      removable: '.trash',
      cellHeight: '3.3rem',
      columnOpts: {
        columnWidth: 100,
        columnMax: 12,
        layout: 'scale',
      },
      minRow: 19,
    };
  }

  loadDashboard() {
    this._dashboardService.get(this.id).subscribe(result => {
      this.dashboard = result;
      this.editFormGroup.controls.name.setValue(this.dashboard.name);
      this.initializeGrid();
    });
  }

  initializeGrid() {
    this.gridWidgets = this.dashboard.tiles.map(tile => ({
      x: tile.x,
      y: tile.y,
      w: tile.width,
      h: tile.height,
      minW: this.constraintsByTileType[tile.itemType].minW,
      minH: this.constraintsByTileType[tile.itemType].minH,
      maxW: this.constraintsByTileType[tile.itemType].maxW,
      maxH: this.constraintsByTileType[tile.itemType].maxH,
      id: tile.id.toString(),
      tile: tile,
    }));
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

    const positionDtos: UpdateTilePositionDto[] = [];
    for (const node of $event.nodes) {
      if (!this.hasTileMoved(node.tile, node.x, node.y, node.w, node.h)) {
        continue;
      }
      const positionDto = new UpdateTilePositionDto();
      positionDto.tileId = node.tile.id;
      positionDto.x = node.x;
      positionDto.y = node.y;
      positionDto.width = node.w;
      positionDto.height = node.h;

      positionDtos.push(positionDto);
    }

    this._dashboardService
      .updateTilePositions(this.id, positionDtos)
      .subscribe(() => {
        for (const node of $event.nodes) {
          const tile = this.dashboard.tiles.find(t => t.id === node.tile.id);
          tile.x = node.x;
          tile.y = node.y;
          tile.width = node.w;
          tile.height = node.h;
        }
      });
  }

  private hasTileMoved(
    tile: DashboardTileDto,
    x: number,
    y: number,
    w: number,
    h: number
  ): boolean {
    return (
      tile.x !== x || tile.y !== y || tile.width !== w || tile.height !== h
    );
  }

  public onGridRemove($event: {event: Event; nodes: CustomGridStackWidget[]}) {
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
