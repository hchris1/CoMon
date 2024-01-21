import {Component, Injector} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {AppComponentBase} from '@shared/app-component-base';
import {
  AssetDto,
  AssetServiceProxy,
  FileParameter,
  GroupPreviewDto,
  GroupServiceProxy,
  ImageDto,
  ImageServiceProxy,
} from '@shared/service-proxies/service-proxies';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {CreatePackageModalComponent} from '@app/edit/create-package-modal/create-package-modal.component';
import {AssistantModalComponent} from '@app/common/assistant-modal/assistant-modal.component';

@Component({
  selector: 'app-asset',
  templateUrl: './asset.component.html',
  animations: [appModuleAnimation()],
})
export class AssetComponent extends AppComponentBase {
  assetId: number;
  asset: AssetDto;

  editMode: boolean = false;
  editTitle: boolean = false;
  editDescription: boolean = false;
  editGroup: boolean = false;
  editFormGroup: FormGroup;
  groups: GroupPreviewDto[];
  createPackageModalRef: BsModalRef;
  assistantModalRef: BsModalRef;

  constructor(
    injector: Injector,
    formBuilder: FormBuilder,
    private _route: ActivatedRoute,
    private _assetService: AssetServiceProxy,
    private _imageService: ImageServiceProxy,
    private _groupService: GroupServiceProxy,
    private _router: Router,
    private _modalService: BsModalService
  ) {
    super(injector);

    this.editFormGroup = formBuilder.group({
      title: ['', [Validators.required, Validators.maxLength(256)]],
      description: ['', []],
      group: ['', []],
    });

    this._route.params.subscribe(params => {
      this.assetId = params['id'];
      this.loadAsset();
    });
  }

  loadAsset() {
    this._assetService.get(this.assetId).subscribe(result => {
      this.asset = result;
    });
  }

  onGroupClick(group: GroupPreviewDto) {
    this._router.navigate(['app', 'overview', group.id]);
  }

  onRootClick() {
    this._router.navigate(['app', 'overview']);
  }

  tableLinkClicked() {
    this._router.navigate(['app', 'table'], {
      queryParams: {assetId: this.assetId},
    });
  }

  editTitleClicked() {
    this.editFormGroup.controls.title.setValue(this.asset.name);
    this.editTitle = true;
  }

  saveTitleClicked() {
    this.editTitle = false;
    this._assetService
      .updateName(this.assetId, this.editFormGroup.controls.title.value)
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
    this._assetService
      .updateDescription(
        this.assetId,
        this.editFormGroup.controls.description.value
      )
      .subscribe(() => {
        this.loadAsset();
      });
  }

  uploadImage(fileParameter: FileParameter) {
    this._assetService
      .uploadImage(this.assetId, fileParameter)
      .subscribe(() => {
        this.loadAsset();
      });
  }

  deleteImage(image: ImageDto) {
    this._imageService.delete(image.id).subscribe(() => {
      this.loadAsset();
    });
  }

  editGroupClick() {
    this._groupService.getAllPreviews().subscribe(result => {
      this.groups = result;

      // Add undefined for root group
      this.groups.unshift(undefined);
      this.editFormGroup.controls.group.setValue(
        this.groups.find(x => x?.id === this.asset.group?.id)
      );

      this.editGroup = true;
    });
  }

  saveGroup() {
    this._assetService
      .updateGroup(this.assetId, this.editFormGroup.controls.group.value?.id)
      .subscribe(() => {
        this.editGroup = false;
        this.loadAsset();
      });
  }

  activateEditMode() {
    this.editMode = true;
  }

  deactivateEditMode() {
    this.editMode = false;
  }

  openCreatePackageModal() {
    this.createPackageModalRef = this._modalService.show(
      CreatePackageModalComponent,
      {
        class: 'modal-lg',
        initialState: {
          assetId: this.assetId,
        },
      }
    );
    this.createPackageModalRef.content.closeBtnName = 'Close';

    this.createPackageModalRef.content.onCreated.subscribe(() => {
      this.loadAsset();
      this.createPackageModalRef.hide();
    });

    this.createPackageModalRef.content.onClose.subscribe(() => {
      this.createPackageModalRef.hide();
    });
  }

  openAssistant() {
    this.assistantModalRef = this._modalService.show(AssistantModalComponent, {
      class: 'modal-lg',
      initialState: {
        assetId: this.assetId,
      },
    });
    this.assistantModalRef.content.closeBtnName = 'Close';

    this.assistantModalRef.content.onClose.subscribe(() => {
      this.assistantModalRef.hide();
    });
  }
}
