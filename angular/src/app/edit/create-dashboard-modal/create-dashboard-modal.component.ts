import {Component, EventEmitter, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {DashboardServiceProxy} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-dashboard-modal',
  templateUrl: './create-dashboard-modal.component.html',
})
export class CreateDashboardModalComponent {
  @Output() onClose = new EventEmitter();

  form: FormGroup;

  constructor(
    formBuilder: FormBuilder,
    private _dashboardService: DashboardServiceProxy,
    private _router: Router
  ) {
    this.form = formBuilder.group({
      name: ['', Validators.required],
    });

    this.form.markAllAsTouched();
  }

  closeClicked() {
    this.onClose.emit();
  }

  onSubmit() {
    if (this.form.valid) {
      this._dashboardService
        .create(this.form.controls.name.value)
        .subscribe(id => {
          this.onClose.emit();
          this._router.navigate(['app', 'dashboard', id]);
        });
    } else {
      this.form.markAllAsTouched();
    }
  }

  onCancel() {
    this.onClose.emit();
  }
}
