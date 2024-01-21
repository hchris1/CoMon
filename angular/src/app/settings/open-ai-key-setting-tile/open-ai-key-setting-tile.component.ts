import {Component, Injector} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {AppComponentBase} from '@shared/app-component-base';
import {
  ChangeOpenAiKeyInput,
  ConfigurationServiceProxy,
  StatusServiceProxy,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-open-ai-key-setting-tile',
  templateUrl: './open-ai-key-setting-tile.component.html',
})
export class OpenAiKeySettingTileComponent extends AppComponentBase {
  openAiKeyFormGroup: FormGroup;

  constructor(
    injector: Injector,
    formBuilder: FormBuilder,
    private _configurationService: ConfigurationServiceProxy,
    private _statusService: StatusServiceProxy
  ) {
    super(injector);

    this.openAiKeyFormGroup = formBuilder.group({
      openAiKey: [''],
    });

    this._configurationService.getOpenAiKey().subscribe(key => {
      this.openAiKeyFormGroup.patchValue({
        openAiKey: key,
      });
    });
  }

  saveOpenAiKeyClicked() {
    const changeOpenAiKeyInput = new ChangeOpenAiKeyInput();
    changeOpenAiKeyInput.openAiKey =
      this.openAiKeyFormGroup.controls.openAiKey.value;

    this._configurationService
      .changeOpenAiKey(changeOpenAiKeyInput)
      .subscribe(() => {
        abp.notify.success(
          this.l('Settings.OpenAiKeySavedDescription'),
          this.l('Settings.OpenAiKeySavedTitle')
        );
      });
  }
}
