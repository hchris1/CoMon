import {Component, EventEmitter, Input, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {
  CreateDashboardTileDto,
  DashboardServiceProxy,
  DashboardTileOptionDto,
  DashboardTileType,
} from '@shared/service-proxies/service-proxies';
import {TILETYPES} from './create-dashboard-tile-modal.constants';

@Component({
  selector: 'app-create-dashboard-tile-modal',
  templateUrl: './create-dashboard-tile-modal.component.html',
})
export class CreateDashboardTileModalComponent {
  @Input() dashboardId: number;
  @Output() onClose = new EventEmitter();
  @Output() onCreated = new EventEmitter();

  form: FormGroup;
  groupTileType = DashboardTileType._0;
  assetTileType = DashboardTileType._1;
  packageTileType = DashboardTileType._2;
  types: {value: DashboardTileType; name: string}[];
  currentType: DashboardTileType;

  options: DashboardTileOptionDto;

  constructor(
    formBuilder: FormBuilder,
    private _dashboardService: DashboardServiceProxy
  ) {
    this.loadOptions();

    this.types = TILETYPES;

    this.form = formBuilder.group({
      type: [0],
      groupId: [''],
      assetId: [''],
      packageId: [''],
    });

    this.form.controls.type.valueChanges.subscribe(value => {
      this.currentType = parseInt(value, 10) as DashboardTileType;

      this.form.controls.groupId.clearValidators();
      this.form.controls.assetId.clearValidators();
      this.form.controls.packageId.clearValidators();

      // Group tile
      if (this.currentType === DashboardTileType._0) {
        this.form.controls.groupId.setValidators([Validators.required]);
      }
      // Asset tile
      else if (this.currentType === DashboardTileType._1) {
        this.form.controls.assetId.setValidators([Validators.required]);
      }
      // Package tile
      else if (this.currentType === DashboardTileType._2) {
        this.form.controls.packageId.setValidators([Validators.required]);
      }

      this.form.controls.groupId.updateValueAndValidity();
      this.form.controls.assetId.updateValueAndValidity();
      this.form.controls.packageId.updateValueAndValidity();
    });

    // Set default values
    this.form.controls.type.setValue(this.groupTileType);

    this.form.markAllAsTouched();
  }

  loadOptions() {
    this._dashboardService.getDashboardTileOptions().subscribe(options => {
      this.options = options;

      if (this.options.groups.length > 0)
        this.form.controls.groupId.setValue(this.options.groups[0].id);

      if (this.options.assets.length > 0)
        this.form.controls.assetId.setValue(this.options.assets[0].id);

      if (this.options.packages.length > 0)
        this.form.controls.packageId.setValue(this.options.packages[0].id);
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const createDto = new CreateDashboardTileDto();
      createDto.itemType = parseInt(
        this.form.controls.type.value,
        10
      ) as DashboardTileType;

      if (createDto.itemType === DashboardTileType._0)
        createDto.itemId = this.form.controls.groupId.value;
      else if (createDto.itemType === DashboardTileType._1)
        createDto.itemId = this.form.controls.assetId.value;
      else if (createDto.itemType === DashboardTileType._2)
        createDto.itemId = this.form.controls.packageId.value;
      else throw new Error('Invalid tile type');

      this._dashboardService
        .addTile(this.dashboardId, createDto)
        .subscribe(() => {
          this.onCreated.emit();
          this.onClose.emit();
        });
    } else {
      this.form.markAllAsTouched();
    }
  }

  onCancel() {
    this.onClose.emit();
  }
}
