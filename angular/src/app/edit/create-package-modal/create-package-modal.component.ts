import {Component, EventEmitter, Input, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {
  CreatePackageDto,
  HttpPackageBodyEncoding,
  HttpPackageMethod,
  HttpPackageSettingsDto,
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

  // Package types
  pingPackageType = PackageType._0;
  httpPackageType = PackageType._1;
  externalPackageType = PackageType._10;
  currentType: PackageType;
  types: {value: PackageType; name: string}[] = [
    {
      value: PackageType._0,
      name: 'Edit.PackageTypePing',
    },
    {
      value: PackageType._1,
      name: 'Edit.PackageTypeHttp',
    },
    {
      value: PackageType._10,
      name: 'Edit.PackageTypeExternal',
    },
  ];

  httpMethods: {value: HttpPackageMethod; name: string}[] = [
    {
      value: HttpPackageMethod._0,
      name: 'Edit.PackageMethodGet',
    },
    {
      value: HttpPackageMethod._1,
      name: 'Edit.PackageMethodPost',
    },
    {
      value: HttpPackageMethod._2,
      name: 'Edit.PackageMethodPut',
    },
    {
      value: HttpPackageMethod._3,
      name: 'Edit.PackageMethodPatch',
    },
    {
      value: HttpPackageMethod._4,
      name: 'Edit.PackageMethodDelete',
    },
  ];
  encodingTypes: {value: HttpPackageBodyEncoding; name: string}[] = [
    {
      value: HttpPackageBodyEncoding._0,
      name: 'Json',
    },
    {
      value: HttpPackageBodyEncoding._1,
      name: 'Xml',
    },
  ];

  constructor(
    formBuilder: FormBuilder,
    private packageService: PackageServiceProxy
  ) {
    this.form = formBuilder.group({
      // Across all package types
      name: ['', Validators.required],
      type: [0, Validators.required],
      // Ping package
      host: [''],
      cycleSeconds: [''],
      // Http package
      url: [''],
      method: [0],
      headers: [''],
      body: [''],
      encoding: [0],
      ignoreSslErrors: [false],
      httpCycleSeconds: [''],
    });

    this.form.controls.type.valueChanges.subscribe(value => {
      this.currentType = parseInt(value, 10) as PackageType;

      this.form.controls.host.clearValidators();
      this.form.controls.cycleSeconds.clearValidators();
      this.form.controls.url.clearValidators();
      this.form.controls.method.clearValidators();
      this.form.controls.httpCycleSeconds.clearValidators();

      // Ping package
      if (this.currentType === PackageType._0) {
        console.log('Setting validators for ping package');
        this.form.controls.host.setValidators([Validators.required]);
        this.form.controls.cycleSeconds.setValidators([
          Validators.required,
          Validators.min(30),
        ]);
        this.form.controls.cycleSeconds.setValue(60);
      }

      // Http package
      if (this.currentType === PackageType._1) {
        console.log('Setting validators for http package');
        this.form.controls.url.setValidators([Validators.required]);
        this.form.controls.method.setValidators([Validators.required]);
        this.form.controls.httpCycleSeconds.setValidators([
          Validators.required,
          Validators.min(30),
        ]);
        this.form.controls.url.setValue('');
        this.form.controls.method.setValue(0);
        this.form.controls.httpCycleSeconds.setValue(60);
      }

      this.form.controls.host.updateValueAndValidity();
      this.form.controls.cycleSeconds.updateValueAndValidity();
      this.form.controls.url.updateValueAndValidity();
      this.form.controls.method.updateValueAndValidity();
      this.form.controls.httpCycleSeconds.updateValueAndValidity();
    });

    this.form.controls.type.setValue(this.pingPackageType);
    this.form.markAllAsTouched();
  }

  closeClicked() {
    this.onClose.emit();
  }

  onSubmit() {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    const createPackageDto = new CreatePackageDto();
    createPackageDto.assetId = this.assetId;
    createPackageDto.name = this.form.controls.name.value;
    createPackageDto.type = this.currentType;

    // Ping package settings
    if (this.currentType === PackageType._0) {
      createPackageDto.pingPackageSettings = new PingPackageSettingsDto();
      createPackageDto.pingPackageSettings.host = this.form.controls.host.value;
      createPackageDto.pingPackageSettings.cycleSeconds =
        this.form.controls.cycleSeconds.value;
    }

    // Http package settings
    if (this.currentType === PackageType._1) {
      createPackageDto.httpPackageSettings = new HttpPackageSettingsDto();
      createPackageDto.httpPackageSettings.url = this.form.controls.url.value;
      createPackageDto.httpPackageSettings.method =
        this.form.controls.method.value;
      createPackageDto.httpPackageSettings.headers =
        this.form.controls.headers.value;
      createPackageDto.httpPackageSettings.body = this.form.controls.body.value;
      createPackageDto.httpPackageSettings.encoding =
        this.form.controls.encoding.value;
      createPackageDto.httpPackageSettings.ignoreSslErrors =
        this.form.controls.ignoreSslErrors.value;
      createPackageDto.httpPackageSettings.cycleSeconds =
        this.form.controls.httpCycleSeconds.value;
    }

    this.packageService.create(createPackageDto).subscribe(id => {
      this.onCreated.emit(id);
      this.onClose.emit();
    });
  }

  onCancel() {
    this.onClose.emit();
  }
}
