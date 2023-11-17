import { Component, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CoMonHubService } from '@app/comon-hub.service';

@Component({
  selector: 'sidebar-logo',
  templateUrl: './sidebar-logo.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarLogoComponent {


  constructor(
    public _coMonHubService: CoMonHubService
  ) { }

}
