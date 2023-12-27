import {Component, Injector} from '@angular/core';
import {
  DashboardDto,
  DashboardPreviewDto,
  DashboardServiceProxy,
} from '@shared/service-proxies/service-proxies';
import {Router} from '@angular/router';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {CreateDashboardModalComponent} from '@app/edit/create-dashboard-modal/create-dashboard-modal.component';
import {AppComponentBase} from '@shared/app-component-base';

@Component({
  selector: 'app-dashboard-overview',
  templateUrl: './dashboard-overview.component.html',
  animations: [appModuleAnimation()],
})
export class DashboardOverviewComponent extends AppComponentBase {
  editMode = false;
  isLoading = false;
  dashboards: DashboardPreviewDto[] = [];
  dashboardIdToDelete: number;

  confirmDeletionModal: BsModalRef;
  createDashboardModal: BsModalRef;

  constructor(
    private _dashboardService: DashboardServiceProxy,
    private _modalService: BsModalService,
    private _router: Router,
    injector: Injector
  ) {
    super(injector);
    this.loadDashboards();
  }

  routeToDashboard(dashboard: DashboardDto) {
    if (this.editMode) return;
    this._router.navigate(['/app/dashboard', dashboard.id]);
  }

  routeToStatisticsDashboard() {
    this._router.navigate(['/app/dashboard/statistics']);
  }

  loadDashboards() {
    this.isLoading = true;
    this._dashboardService.getAll().subscribe(result => {
      this.dashboards = result.sort((a, b) => a.name.localeCompare(b.name));
      this.isLoading = false;
    });
  }

  activateEditMode() {
    this.editMode = true;
  }

  deactivateEditMode() {
    this.editMode = false;
  }

  deleteDashboardClicked(dashboard: DashboardDto) {
    this.message.confirm(
      this.l('Dashboard.DeleteConfirmationMessage'),
      this.l('Dashboard.DeleteConfirmationTitle'),
      isConfirmed => {
        if (isConfirmed) {
          this._dashboardService.delete(dashboard.id).subscribe(() => {
            this.loadDashboards();
            this.notify.success(
              this.l('Dashboard.DeleteSuccessMessage'),
              dashboard.name
            );
          });
        }
      }
    );
  }

  onCreateDashboardModal() {
    this.createDashboardModal = this._modalService.show(
      CreateDashboardModalComponent,
      {
        class: 'modal-lg',
      }
    );
    this.createDashboardModal.content.closeBtnName = 'Close';

    this.createDashboardModal.content.onClose.subscribe(() => {
      this.createDashboardModal.hide();
    });
  }
}
