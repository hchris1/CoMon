import { ChangeDetectorRef, Component, Injector, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { AssetDto, AssetServiceProxy, FileParameter, GroupPreviewDto, GroupServiceProxy, ImageDto, ImageServiceProxy, StatusPreviewDto, StatusServiceProxy } from '@shared/service-proxies/service-proxies';
import { CoMonHubService } from '../comon-hub.service';
import { DynamicStylesHelper } from '@shared/helpers/DynamicStylesHelper';
import { RoutingHelper } from '@shared/helpers/RoutingHelper';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CreatePackageModalComponent } from '@app/edit/create-package-modal/create-package-modal.component';

@Component({
  selector: 'app-asset',
  templateUrl: './asset.component.html',
  animations: [appModuleAnimation()],
})
export class AssetComponent extends AppComponentBase implements OnDestroy {

  assetId: number;
  asset: AssetDto;
  statusPreviews: StatusPreviewDto[];
  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;

  editMode: boolean = false;
  editTitle: boolean = false;
  editDescription: boolean = false;
  editGroup: boolean = false;
  editFormGroup: FormGroup;
  groups: GroupPreviewDto[];
  createPackageModalRef: BsModalRef;

  constructor(
    injector: Injector,
    changeDetector: ChangeDetectorRef,
    formBuilder: FormBuilder,
    private _statusService: StatusServiceProxy,
    private _route: ActivatedRoute,
    private _assetService: AssetServiceProxy,
    private _imageService: ImageServiceProxy,
    private _groupService: GroupServiceProxy,
    private _coMonHubService: CoMonHubService,
    private _router: Router,
    private _modalService: BsModalService
  ) {
    super(injector);

    this._route.queryParams.subscribe(params => {
      this.editMode = params['editMode'] === 'true';
    });

    this.editFormGroup = formBuilder.group({
      title: ['', [Validators.required]],
      description: ['', []],
      group: ['', []]
    });

    this.statusChangeSubscription = this._coMonHubService.statusUpdate.subscribe((update) => {
      if (parseInt(this.assetId.toString(), 10) === update.assetId) {
        this.loadStatusPreviews();
        changeDetector.detectChanges();
      }
    });

    this._route.params.subscribe(params => {
      this.assetId = params['id'];
      this.loadAsset();
      this.loadStatusPreviews();
    });

    this.connectionEstablishedSubscription = this._coMonHubService.connectionEstablished.subscribe((established) => {
      if (established)
        this.loadStatusPreviews();
    });
  }

  ngOnDestroy(): void {
    this.statusChangeSubscription.unsubscribe();
    this.connectionEstablishedSubscription.unsubscribe();
  }

  loadAsset() {
    this._assetService.get(this.assetId).subscribe((result) => {
      this.asset = result;
    });
  }

  loadStatusPreviews() {
    this._statusService.getLatestStatusPreviews(this.assetId).subscribe((result) => {
      this.statusPreviews = result;
    });
  }

  onGroupClick(group: GroupPreviewDto) {
    this._router.navigate(['app', 'overview', group.id], RoutingHelper.buildEditModeQueryParams(this.editMode));
  }

  onRootClick() {
    this._router.navigate(['app', 'overview'], RoutingHelper.buildEditModeQueryParams(this.editMode));
  }

  tableLinkClicked() {
    this._router.navigate(['app', 'table'], { queryParams: { assetId: this.assetId } });
  }

  getEmoji() {
    const worstCriticality = Math.max(...this.statusPreviews.map((x) => x.criticality));
    return DynamicStylesHelper.getEmoji(worstCriticality);
  }

  editTitleClicked() {
    this.editFormGroup.controls.title.setValue(this.asset.name);
    this.editTitle = true;
  }

  saveTitleClicked() {
    this.editTitle = false;
    this._assetService.updateName(this.assetId, this.editFormGroup.controls.title.value)
      .subscribe(() => {
        this.loadAsset();
      });
  }

  editDescriptionClicked() {
    this.editFormGroup.controls.description.setValue(this.asset.description);
    this.editDescription = true;
  }

  saveDescription() {
    this.editDescription = false;
    this._assetService.updateDescription(this.assetId, this.editFormGroup.controls.description.value)
      .subscribe(() => {
        this.loadAsset();
      });
  }

  uploadImage(fileParameter: FileParameter) {
    this._assetService.uploadImage(this.assetId, fileParameter).subscribe(() => {
      this.loadAsset();
    });
  }

  deleteImage(image: ImageDto) {
    this._imageService.delete(image.id).subscribe(() => {
      this.loadAsset();
    });
  }

  editGroupClick() {
    this._groupService.getAll().subscribe((result) => {
      this.groups = result;

      // Add undefined for root group
      this.groups.unshift(undefined);
      this.editFormGroup.controls.group.setValue(
        this.groups.find((x) => x?.id === this.asset.group?.id)
      );

      this.editGroup = true;
    });
  }

  saveGroup() {
    this._assetService.updateGroup(this.assetId, this.editFormGroup.controls.group.value?.id)
      .subscribe(() => {
        this.editGroup = false;
        this.loadAsset();
      });
  }

  activateEditMode() {
    this._router.navigate([], {
      relativeTo: this._route,
      queryParams: RoutingHelper.buildEditModeQueryParams(true),
      queryParamsHandling: 'merge'
    });
    this.editMode = true;
  }

  deactivateEditMode() {
    this._router.navigate([], {
      relativeTo: this._route,
      queryParams: RoutingHelper.buildEditModeQueryParams(false),
      queryParamsHandling: 'merge'
    });
    this.editMode = false;
  }

  openCreatePackageModal() {
    this.createPackageModalRef = this._modalService.show(CreatePackageModalComponent,
      {
        class: 'modal-lg',
        initialState: {
          assetId: this.assetId
        }
      });
    this.createPackageModalRef.content.closeBtnName = 'Close';

    this.createPackageModalRef.content.onCreated.subscribe(() => {
      this.loadStatusPreviews();
      this.createPackageModalRef.hide();
    });

    this.createPackageModalRef.content.onClose.subscribe(() => {
      this.createPackageModalRef.hide();
    });
  }
}
