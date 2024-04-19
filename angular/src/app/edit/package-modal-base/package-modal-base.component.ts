import {Component, EventEmitter, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {PackageType} from '@shared/service-proxies/service-proxies';
import {
  ENCODINGTYPES,
  HTTPMETHODS,
  PACKAGETYPES,
  RTSPMETHODS,
} from './package-modal-base.constants';

@Component({
  selector: 'app-package-modal-base',
  templateUrl: './package-modal-base.component.html',
})
export class PackageModalBase {
  @Output() onClose = new EventEmitter();
  @Output() onCreated = new EventEmitter<number>();

  form: FormGroup;

  currentType: PackageType;
  pingPackageType = PackageType._0;
  httpPackageType = PackageType._1;
  rtspPackageType = PackageType._2;
  externalPackageType = PackageType._10;

  types = PACKAGETYPES;
  httpMethods = HTTPMETHODS;
  encodingTypes = ENCODINGTYPES;
  rtspMethods = RTSPMETHODS;

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      name: ['', Validators.required],
      type: [0, Validators.required],
      host: [''],
      cycleSeconds: [''],
      url: [''],
      method: [0],
      headers: [''],
      body: [''],
      encoding: [0],
      ignoreSslErrors: [false],
      rtspMethod: [0],
    });

    this.form.controls.type.valueChanges.subscribe(value => {
      this.currentType = parseInt(value, 10) as PackageType;

      this.form.controls.host.clearValidators();
      this.form.controls.cycleSeconds.clearValidators();
      this.form.controls.url.clearValidators();
      this.form.controls.method.clearValidators();
      this.form.controls.rtspMethod.clearValidators();

      // Ping package
      if (this.currentType === PackageType._0) {
        this.form.controls.host.setValidators([Validators.required]);
        this.form.controls.cycleSeconds.setValidators([
          Validators.required,
          Validators.min(30),
        ]);
        this.form.controls.cycleSeconds.setValue(60);
      }

      // Http package
      if (this.currentType === PackageType._1) {
        this.form.controls.url.setValidators([Validators.required]);
        this.form.controls.method.setValidators([Validators.required]);
        this.form.controls.cycleSeconds.setValidators([
          Validators.required,
          Validators.min(30),
        ]);
        this.form.controls.url.setValue('');
        this.form.controls.method.setValue(0);
        this.form.controls.cycleSeconds.setValue(60);
      }

      // Rtsp package
      if (this.currentType === PackageType._2) {
        this.form.controls.url.setValidators([Validators.required]);
        this.form.controls.rtspMethod.setValidators([Validators.required]);
        this.form.controls.cycleSeconds.setValidators([
          Validators.required,
          Validators.min(30),
        ]);
        this.form.controls.url.setValue('');
        this.form.controls.rtspMethod.setValue(0);
        this.form.controls.cycleSeconds.setValue(60);
      }

      this.form.controls.host.updateValueAndValidity();
      this.form.controls.cycleSeconds.updateValueAndValidity();
      this.form.controls.url.updateValueAndValidity();
      this.form.controls.method.updateValueAndValidity();
      this.form.controls.rtspMethod.updateValueAndValidity();
    });

    this.form.controls.type.setValue(this.pingPackageType);
    this.form.markAllAsTouched();
  }

  closeClicked() {
    this.onClose.emit();
  }

  onSubmit() {}

  onCancel() {
    this.onClose.emit();
  }
}
