import {Component, Injector} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {AppComponentBase} from '@shared/app-component-base';
import {
  ChangeRetentionDaysInput,
  ConfigurationServiceProxy,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  animations: [appModuleAnimation()],
})
export class SettingsComponent extends AppComponentBase {
  retentionFormGroup: FormGroup;

  constructor(
    injector: Injector,
    formBuilder: FormBuilder,
    private _configurationService: ConfigurationServiceProxy
  ) {
    super(injector);
    this.retentionFormGroup = formBuilder.group({
      retentionDays: [30, [Validators.required, Validators.min(-1)]],
    });

    this._configurationService.getRetentionDays().subscribe(days => {
      this.retentionFormGroup.patchValue({
        retentionDays: days,
      });
    });
  }

  saveRetentionPolicyClicked() {
    const changeRetentionDaysInput = new ChangeRetentionDaysInput();
    changeRetentionDaysInput.days =
      this.retentionFormGroup.controls.retentionDays.value;

    this._configurationService
      .changeRetentionDays(changeRetentionDaysInput)
      .subscribe(() => {
        abp.notify.success(
          this.l('Settings.RetentionDaysSavedDescription'),
          this.l('Settings.RetentionDaysSavedTitle')
        );
      });
  }
}
