import {Component, EventEmitter, Input, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {
  AssetServiceProxy,
  CreateAssetDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-asset-modal',
  templateUrl: './create-asset-modal.component.html',
})
export class CreateAssetModalComponent {
  @Input() groupId: number;
  @Output() onClose = new EventEmitter();

  form: FormGroup;

  constructor(
    formBuilder: FormBuilder,
    private _assetService: AssetServiceProxy,
    private _router: Router
  ) {
    this.form = formBuilder.group({
      name: ['', Validators.required],
      description: [''],
    });

    this.form.markAllAsTouched();
  }

  closeClicked() {
    this.onClose.emit();
  }

  onSubmit() {
    if (this.form.valid) {
      const createAssetDto = new CreateAssetDto();
      createAssetDto.name = this.form.controls.name.value;
      createAssetDto.description = this.form.controls.description.value;
      createAssetDto.groupId = this.groupId;
      this._assetService.create(createAssetDto).subscribe(id => {
        this.onClose.emit();
        this._router.navigate(['app', 'overview', 'assets', id]);
      });
    } else {
      this.form.markAllAsTouched();
    }
  }

  onCancel() {
    this.onClose.emit();
  }
}
