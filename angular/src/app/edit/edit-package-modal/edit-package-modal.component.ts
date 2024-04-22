import {Component, EventEmitter, Input, Output} from '@angular/core';
import {FormBuilder, FormControl, Validators} from '@angular/forms';
import {
  AssetPreviewDto,
  AssetServiceProxy,
  HttpPackageSettingsDto,
  PackageServiceProxy,
  PackageType,
  PingPackageSettingsDto,
  RtspPackageSettingsDto,
  UpdatePackageDto,
} from '@shared/service-proxies/service-proxies';
import {PackageModalBase} from '../package-modal-base/package-modal-base.component';

@Component({
  selector: 'app-edit-package-modal',
  templateUrl: './edit-package-modal.component.html',
})
export class EditPackageModalComponent extends PackageModalBase {
  @Input() packageId: number;
  @Output() onClose = new EventEmitter();
  @Output() onEdited = new EventEmitter<number>();

  assetId: number;
  assets: AssetPreviewDto[];
  isLoading = true;

  constructor(
    private _assetService: AssetServiceProxy,
    private _packageService: PackageServiceProxy,
    formBuilder: FormBuilder
  ) {
    super(formBuilder);

    this.form.addControl('asset', new FormControl('', Validators.required));
  }

  ngOnInit() {
    this._packageService.get(this.packageId).subscribe(result => {
      this.assetId = result.assetId;

      this._assetService.getAllPreviews().subscribe(result => {
        this.assets = result;
        this.form.controls.asset.setValue(
          this.assets.find(x => x.id === this.assetId)
        );
        this.isLoading = false;
      });

      this.form.controls.name.setValue(result.name);
      this.form.controls.type.setValue(result.type);

      if (result.type === PackageType._0) {
        this.form.controls.host.setValue(result.pingPackageSettings.host);
        this.form.controls.cycleSeconds.setValue(
          result.pingPackageSettings.cycleSeconds
        );
      }

      if (result.type === PackageType._1) {
        this.form.controls.url.setValue(result.httpPackageSettings.url);
        this.form.controls.method.setValue(result.httpPackageSettings.method);
        this.form.controls.headers.setValue(result.httpPackageSettings.headers);
        this.form.controls.body.setValue(result.httpPackageSettings.body);
        this.form.controls.encoding.setValue(
          result.httpPackageSettings.encoding
        );
        this.form.controls.ignoreSslErrors.setValue(
          result.httpPackageSettings.ignoreSslErrors
        );
        this.form.controls.cycleSeconds.setValue(
          result.httpPackageSettings.cycleSeconds
        );
      }

      if (result.type === PackageType._2) {
        this.form.controls.url.setValue(result.rtspPackageSettings.url);
        this.form.controls.rtspMethod.setValue(
          result.rtspPackageSettings.method
        );
        this.form.controls.cycleSeconds.setValue(
          result.rtspPackageSettings.cycleSeconds
        );
      }

      this.currentType = result.type;
      this.form.markAllAsTouched();
    });
  }

  onSubmit(enqueueCheck: boolean = false) {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    const pack = new UpdatePackageDto();
    pack.id = this.packageId;
    pack.assetId = this.form.controls.asset.value.id;
    pack.name = this.form.controls.name.value;
    pack.type = this.form.controls.type.value;

    if (this.currentType === PackageType._0) {
      pack.pingPackageSettings = new PingPackageSettingsDto();
      pack.pingPackageSettings.host = this.form.controls.host.value;
      pack.pingPackageSettings.cycleSeconds =
        this.form.controls.cycleSeconds.value;
    }

    if (this.currentType === PackageType._1) {
      pack.httpPackageSettings = new HttpPackageSettingsDto();
      pack.httpPackageSettings.url = this.form.controls.url.value;
      pack.httpPackageSettings.method = this.form.controls.method.value;
      pack.httpPackageSettings.headers = this.form.controls.headers.value;
      pack.httpPackageSettings.body = this.form.controls.body.value;
      pack.httpPackageSettings.encoding = this.form.controls.encoding.value;
      pack.httpPackageSettings.ignoreSslErrors =
        this.form.controls.ignoreSslErrors.value;
      pack.httpPackageSettings.cycleSeconds =
        this.form.controls.cycleSeconds.value;
    }

    if (this.currentType === PackageType._2) {
      pack.rtspPackageSettings = new RtspPackageSettingsDto();
      pack.rtspPackageSettings.url = this.form.controls.url.value;
      pack.rtspPackageSettings.method = this.form.controls.rtspMethod.value;
      pack.rtspPackageSettings.cycleSeconds =
        this.form.controls.cycleSeconds.value;
    }

    this._packageService.update(enqueueCheck, pack).subscribe(() => {
      this.onEdited.emit(this.packageId);
      this.onClose.emit();
    });
  }
}
