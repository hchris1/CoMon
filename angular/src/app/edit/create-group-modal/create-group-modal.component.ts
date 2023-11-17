import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CreateGroupDto, GroupServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-group-modal',
  templateUrl: './create-group-modal.component.html'
})
export class CreateGroupModalComponent {
  @Input() parentGroupId: number = undefined;
  @Output() onClose = new EventEmitter();

  form: FormGroup;

  constructor(
    formBuilder: FormBuilder,
    private _groupService: GroupServiceProxy,
    private _router: Router
  ) {
    this.form = formBuilder.group({
      name: ['', Validators.required]
    });
  }

  closeClicked() {
    this.onClose.emit();
  }

  onSubmit() {
    if (this.form.valid) {
      const createGroupDto = new CreateGroupDto();
      createGroupDto.name = this.form.controls.name.value;
      createGroupDto.parentId = this.parentGroupId;
      this._groupService.create(createGroupDto).subscribe((id) => {
        this.onClose.emit();
        this._router.navigate(['app', 'overview', id]);
      });
    } else {
      this.form.markAllAsTouched();
    }
  }

  onCancel() {
    this.onClose.emit();
  }
}
