import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CoMonHubService } from '@app/comon-hub.service';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { KPIDto, PackagePreviewDto, PackageServiceProxy } from '@shared/service-proxies/service-proxies';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-package',
  templateUrl: './package.component.html',
  animations: [appModuleAnimation()],
})
export class PackageComponent {

  packageId: number;
  package: PackagePreviewDto;
  statusChanged = false;
  triggerReload: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  triggerRender: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(
    route: ActivatedRoute,
    private _packageService: PackageServiceProxy,
    comonHubService: CoMonHubService
  ) {
    route.params.subscribe(params => {
      this.packageId = +params['id'];
      this.loadPackage();
    });

    comonHubService.statusUpdate.subscribe((update) => {
      if (update.packageId == this.packageId) {
        this.statusChanged = true;
      }
    });

  }

  loadPackage() {
    this._packageService.getPreview(this.packageId).subscribe((data: PackagePreviewDto) => {
      this.package = data;
    });
  }

  refresh() {
    this.triggerReload.next(true);
    this.statusChanged = false;
  }

  tabChanged() {
    this.triggerRender.next(true);
  }
}
