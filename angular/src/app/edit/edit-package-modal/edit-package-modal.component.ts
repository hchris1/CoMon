import {Component, EventEmitter, Input, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {
  AssetPreviewDto,
  AssetServiceProxy,
  HttpPackageBodyEncoding,
  HttpPackageMethod,
  HttpPackageSettingsDto,
  PackageServiceProxy,
  PackageType,
  PingPackageSettingsDto,
  UpdatePackageDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-edit-package-modal',
  templateUrl: './edit-package-modal.component.html',
})
export class EditPackageModalComponent {
  @Input() packageId: number;
  @Output() onClose = new EventEmitter();
  @Output() onEdited = new EventEmitter<number>();

  assetId: number;
  form: FormGroup;
  pingPackageType = PackageType._0;
  httpPackageType = PackageType._1;
  externalPackageType = PackageType._10;
  types: {value: PackageType; name: string}[];
  currentType: PackageType;
  assets: AssetPreviewDto[];

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
    private _assetService: AssetServiceProxy,
    private _packageService: PackageServiceProxy,
    private _formBuilder: FormBuilder
  ) {
    this.form = this._formBuilder.group({
      name: ['', Validators.required],
      asset: ['', Validators.required],
      type: ['', Validators.required],
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
  }

  ngOnInit() {
    this._packageService.get(this.packageId).subscribe(result => {
      this.assetId = result.assetId;

      this._assetService.getAll().subscribe(result => {
        this.assets = result;
        this.form.controls.asset.setValue(
          this.assets.find(x => x.id === this.assetId)
        );
      });

      this.form.controls.name.setValue(result.name);
      this.form.controls.type.setValue(result.type);

      if (result.type === PackageType._0) {
        this.form.controls.host.setValue(result.pingPackageSettings.host);
        this.form.controls.host.setValidators([Validators.required]);
        this.form.controls.cycleSeconds.setValue(
          result.pingPackageSettings.cycleSeconds
        );
        this.form.controls.cycleSeconds.setValidators([
          Validators.required,
          Validators.min(30),
        ]);
      }

      if (result.type === PackageType._1) {
        this.form.controls.url.setValue(result.httpPackageSettings.url);
        this.form.controls.url.setValidators([Validators.required]);

        this.form.controls.method.setValue(result.httpPackageSettings.method);
        this.form.controls.method.setValidators([Validators.required]);

        this.form.controls.headers.setValue(result.httpPackageSettings.headers);
        this.form.controls.body.setValue(result.httpPackageSettings.body);
        this.form.controls.encoding.setValue(
          result.httpPackageSettings.encoding
        );
        this.form.controls.ignoreSslErrors.setValue(
          result.httpPackageSettings.ignoreSslErrors
        );
        this.form.controls.httpCycleSeconds.setValue(
          result.httpPackageSettings.cycleSeconds
        );
        this.form.controls.httpCycleSeconds.setValidators([
          Validators.required,
          Validators.min(30),
        ]);
      }

      this.currentType = result.type;

      this.form.markAllAsTouched();
    });
  }

  onSubmit() {
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
        this.form.controls.httpCycleSeconds.value;
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
