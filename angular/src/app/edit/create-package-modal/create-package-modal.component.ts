import {Component, EventEmitter, Input, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {
  CreatePackageDto,
  PackageServiceProxy,
  PackageType,
  PingPackageSettingsDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-package-modal',
  templateUrl: './create-package-modal.component.html',
})
export class CreatePackageModalComponent {
  @Input() assetId: number;
  @Output() onClose = new EventEmitter();
  @Output() onCreated = new EventEmitter<number>();

  form: FormGroup;
  pingPackageType = PackageType._0;
  externalPackageType = PackageType._10;
  types: {value: PackageType; name: string}[];
  currentType: PackageType;

  constructor(
    formBuilder: FormBuilder,
    private packageService: PackageServiceProxy
  ) {
    this.types = [
      {
        value: PackageType._0,
        name: 'Edit.PackageTypePing',
      },
      {
        value: PackageType._10,
        name: 'Edit.PackageTypeExternal',
      },
    ];

    this.form = formBuilder.group({
      name: ['', Validators.required],
      type: [0, Validators.required],
      host: [''],
      cycleSeconds: [''],
    });

    this.form.controls.type.valueChanges.subscribe(value => {
      this.currentType = parseInt(value, 10) as PackageType;

      this.form.controls.host.clearValidators();
      this.form.controls.cycleSeconds.clearValidators();
      this.form.controls.host.setValue('');
      this.form.controls.cycleSeconds.setValue('');

      // Ping package
      if (this.currentType === PackageType._0) {
        this.form.controls.host.setValidators([Validators.required]);
        this.form.controls.cycleSeconds.setValidators([Validators.required]);
        this.form.controls.cycleSeconds.setValue(60);
      }
    });

    // Set default values
    this.form.controls.type.setValue(this.pingPackageType);
    this.form.controls.cycleSeconds.setValue(60);
  }

  closeClicked() {
    this.onClose.emit();
  }

  onSubmit() {
    if (this.form.valid) {
      const createPackageDto = new CreatePackageDto();
      createPackageDto.assetId = this.assetId;
      createPackageDto.name = this.form.controls.name.value;
      createPackageDto.type = this.currentType;

      // Ping package settings
      if (this.currentType === PackageType._0) {
        createPackageDto.pingPackageSettings = new PingPackageSettingsDto();
        createPackageDto.pingPackageSettings.host =
          this.form.controls.host.value;
        createPackageDto.pingPackageSettings.cycleSeconds =
          this.form.controls.cycleSeconds.value;
      }

      this.packageService.create(createPackageDto).subscribe(id => {
        this.onCreated.emit(id);
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
