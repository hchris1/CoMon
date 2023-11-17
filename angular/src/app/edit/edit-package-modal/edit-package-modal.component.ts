import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AssetPreviewDto, AssetServiceProxy, PackageServiceProxy, PackageType, PingPackageSettingsDto, UpdatePackageDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-edit-package-modal',
  templateUrl: './edit-package-modal.component.html'
})
export class EditPackageModalComponent {
  @Input() packageId: number;
  @Output() onClose = new EventEmitter();
  @Output() onEdited = new EventEmitter<number>();

  assetId: number;
  form: FormGroup;
  pingPackageType = PackageType._0;
  externalPackageType = PackageType._10;
  types: { value: PackageType, name: string }[];
  currentType: PackageType;
  assets: AssetPreviewDto[];

  constructor(
    private _assetService: AssetServiceProxy,
    private _packageService: PackageServiceProxy,
    private _formBuilder: FormBuilder
  ) {
    this.form = this._formBuilder.group({
      name: ['', Validators.required],
      asset: ['', Validators.required],
      type: ['', Validators.required],
      host: [''],
      cycleSeconds: ['']
    });
  }

  ngOnInit() {
    this._packageService.get(this.packageId).subscribe(result => {
      this.assetId = result.assetId;

      this._assetService.getAll().subscribe(result => {
        this.assets = result;
        this.form.controls.asset.setValue(this.assets.find(x => x.id == this.assetId));
      });

      this.form.controls.name.setValue(result.name);
      this.form.controls.type.setValue(result.type);

      if (result.type === PackageType._0) {
        this.form.controls.host.setValue(result.pingPackageSettings.host);
        this.form.controls.cycleSeconds.setValue(result.pingPackageSettings.cycleSeconds);
        this.form.controls.host.setValidators([Validators.required]);
        this.form.controls.cycleSeconds.setValidators([Validators.required]);
      }

      this.currentType = result.type;
    });
  }

  onSubmit() {
    const pack = new UpdatePackageDto();
    pack.id = this.packageId;
    pack.assetId = this.form.controls.asset.value.id;
    pack.name = this.form.controls.name.value;
    pack.type = this.form.controls.type.value;

    if (this.currentType == PackageType._0) {
      pack.pingPackageSettings = new PingPackageSettingsDto();
      pack.pingPackageSettings.host = this.form.controls.host.value;
      pack.pingPackageSettings.cycleSeconds = this.form.controls.cycleSeconds.value;
    }

    this._packageService.update(pack).subscribe(() => {
      this.onEdited.emit(this.packageId);
      this.onClose.emit();
    });
  }

  onCancel() {
    this.onClose.emit();
  }
}
