import {Component, TemplateRef} from '@angular/core';
import {
  DashboardDto,
  DashboardPreviewDto,
  DashboardServiceProxy,
} from '@shared/service-proxies/service-proxies';
import {ActivatedRoute, Router} from '@angular/router';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {CreateDashboardModalComponent} from '@app/edit/create-dashboard-modal/create-dashboard-modal.component';
import {RoutingHelper} from '@shared/helpers/RoutingHelper';

@Component({
  selector: 'app-dashboard-overview',
  templateUrl: './dashboard-overview.component.html',
  animations: [appModuleAnimation()],
})
export class DashboardOverviewComponent {
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
    private _route: ActivatedRoute
  ) {
    this.loadDashboards();
  }

  routeToDashboard(dashboard: DashboardDto) {
    if (this.editMode) return;
    this._router.navigate(['/app/dashboard', dashboard.id]);
  }

  loadDashboards() {
    this.isLoading = true;
    this._dashboardService.getAll().subscribe(result => {
      this.dashboards = result.sort((a, b) => a.name.localeCompare(b.name));
      this.isLoading = false;
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

  /* eslint-disable @typescript-eslint/no-explicit-any */
  openDeletionModal(template: TemplateRef<any>, dashboard: DashboardDto) {
    this.dashboardIdToDelete = dashboard.id;
    this.confirmDeletionModal = this._modalService.show(template, {
      class: 'modal-sm',
    });
  }

  confirmDeletion() {
    this._dashboardService.delete(this.dashboardIdToDelete).subscribe(() => {
      this.confirmDeletionModal.hide();
      this.loadDashboards();
    });
  }

  cancelDeletion() {
    this.confirmDeletionModal.hide();
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
