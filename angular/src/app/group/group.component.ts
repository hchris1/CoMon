import {Component, Input} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {CreateAssetModalComponent} from '@app/edit/create-asset-modal/create-asset-modal.component';
import {CreateGroupModalComponent} from '@app/edit/create-group-modal/create-group-modal.component';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {
  AssetDto,
  GroupDto,
  GroupPreviewDto,
  GroupServiceProxy,
  StatusDto,
} from '@shared/service-proxies/service-proxies';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-group',
  templateUrl: './group.component.html',
  animations: [appModuleAnimation()],
})
export class GroupComponent {
  @Input() editMode: boolean = false;

  groupId: number;
  group: GroupDto;
  isRoot: boolean = false;
  editFormGroup: FormGroup;
  editName: boolean = false;
  editGroup: boolean = false;
  groups: GroupPreviewDto[];

  createGroupModalRef: BsModalRef;
  createAssetModalRef: BsModalRef;

  constructor(
    route: ActivatedRoute,
    private _router: Router,
    private _groupService: GroupServiceProxy,
    private _formBuilder: FormBuilder,
    private _modalService: BsModalService
  ) {
    this.editFormGroup = this._formBuilder.group({
      name: ['', []],
      group: ['', []],
    });

    route.params.subscribe(params => {
      this.groupId = params['id'];
      this.loadGroup();
    });
  }

  loadGroup() {
    if (this.groupId) {
      this._groupService.get(this.groupId).subscribe(result => {
        this.group = result;
        this.isRoot = false;
      });
    } else {
      this._groupService.getRoot().subscribe(result => {
        this.group = result;
        this.isRoot = true;
      });
    }
  }

  getTitleClass(asset: AssetDto) {
    const worstCriticality = DynamicStylesHelper.getWorstCriticality(asset);
    return DynamicStylesHelper.getBackgroundClass(worstCriticality);
  }

  getBadgeClass(status: StatusDto) {
    return DynamicStylesHelper.getBadgeClass(status.criticality);
  }

  routeToAsset(asset: AssetDto) {
    this._router.navigate(['app', 'overview', 'assets', asset.id]);
  }

  routeToGroup(group: GroupPreviewDto) {
    this._router.navigate(['app', 'overview', group.id]);
  }

  routeToTable() {
    this._router.navigate(['app', 'table'], {
      queryParams: {
        groupId: this.groupId,
      },
      queryParamsHandling: 'merge',
    });
  }

  routeToParent() {
    this.editName = false;
    this.editGroup = false;
    if (this.group.parent) {
      this._router.navigate(['app', 'overview', this.group.parent.id]);
    } else {
      this._router.navigate(['app', 'overview']);
    }
  }

  activateEditName() {
    this.editFormGroup.controls.name.setValue(this.group.name);
    this.editName = true;
  }

  saveName() {
    this.editName = false;
    this._groupService
      .updateName(this.groupId, this.editFormGroup.controls.name.value)
      .subscribe(() => {
        this.loadGroup();
      });
  }

  activateEditGroup() {
    this._groupService.getAll().subscribe(result => {
      this.groups = result
        .filter(x => x.id !== this.group.id)
        .filter(x => !this.hasParentWithId(x, this.group.id));

      // Add undefined for root group
      this.groups.unshift(undefined);
      this.editFormGroup.controls.group.setValue(undefined);

      this.editGroup = true;
    });
  }

  hasParentWithId(group: GroupPreviewDto, id: number) {
    if (!group.parent) {
      return false;
    }

    if (group.parent.id === id) {
      return true;
    }

    return this.hasParentWithId(group.parent, id);
  }

  saveGroup() {
    this._groupService
      .updateParent(this.groupId, this.editFormGroup.controls.group.value?.id)
      .subscribe(() => {
        this.editGroup = false;
        this.loadGroup();
      });
  }

  openCreateGroupModal() {
    this.createGroupModalRef = this._modalService.show(
      CreateGroupModalComponent,
      {
        class: 'modal-lg',
        initialState: {
          parentGroupId: this.groupId,
        },
      }
    );
    this.createGroupModalRef.content.closeBtnName = 'Close';

    this.createGroupModalRef.content.onClose.subscribe(() => {
      this.createGroupModalRef.hide();
    });
  }

  openCreateAssetModal() {
    this.createAssetModalRef = this._modalService.show(
      CreateAssetModalComponent,
      {
        class: 'modal-lg',
        initialState: {
          groupId: this.groupId,
        },
      }
    );
    this.createAssetModalRef.content.closeBtnName = 'Close';

    this.createAssetModalRef.content.onClose.subscribe(() => {
      this.createAssetModalRef.hide();
    });
  }

  activateEditMode() {
    this.editMode = true;
  }

  deactivateEditMode() {
    this.editMode = false;
  }
}
