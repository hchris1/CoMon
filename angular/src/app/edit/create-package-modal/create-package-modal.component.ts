import {Component, Input} from '@angular/core';
import {FormBuilder} from '@angular/forms';
import {
  CreatePackageDto,
  HttpPackageSettingsDto,
  PackageServiceProxy,
  PackageType,
  PingPackageSettingsDto,
  RtspPackageSettingsDto,
} from '@shared/service-proxies/service-proxies';
import {PackageModalBase} from '../package-modal-base/package-modal-base.component';

@Component({
  selector: 'app-create-package-modal',
  templateUrl: './create-package-modal.component.html',
})
export class CreatePackageModalComponent extends PackageModalBase {
  @Input() assetId: number;

  constructor(
    formBuilder: FormBuilder,
    private packageService: PackageServiceProxy
  ) {
    super(formBuilder);
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
        this.form.controls.cycleSeconds.value;
    }

    // Rtsp package settings
    if (this.currentType === PackageType._2) {
      createPackageDto.rtspPackageSettings = new RtspPackageSettingsDto();
      createPackageDto.rtspPackageSettings.url = this.form.controls.url.value;
      createPackageDto.rtspPackageSettings.method =
        this.form.controls.rtspMethod.value;
      createPackageDto.rtspPackageSettings.cycleSeconds =
        this.form.controls.cycleSeconds.value;
    }

    this.packageService.create(createPackageDto).subscribe(id => {
      this.onCreated.emit(id);
      this.onClose.emit();
    });
  }
}
