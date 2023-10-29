import {
  Component,
  OnInit,
  ViewEncapsulation,
  Injector,
  Renderer2,
} from '@angular/core';
import {AppComponentBase} from '@shared/app-component-base';

@Component({
  templateUrl: './account.component.html',
  encapsulation: ViewEncapsulation.None,
})
export class AccountComponent extends AppComponentBase implements OnInit {
  constructor(
    injector: Injector,
    private renderer: Renderer2
  ) {
    super(injector);
  }

  showTenantChange(): boolean {
    return abp.multiTenancy.isEnabled;
  }

  ngOnInit(): void {
    this.renderer.addClass(document.body, 'login-page');
    this.renderer.setStyle(
      document.body,
      'background',
      'linear-gradient(0deg, #D9AFD9 0%, #97D9E1 100%)'
    );
  }
}
