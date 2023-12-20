import {Component, Input, OnInit} from '@angular/core';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {
  KPIDto,
  PackageServiceProxy,
} from '@shared/service-proxies/service-proxies';
import {BehaviorSubject} from 'rxjs';

@Component({
  selector: 'app-package-chart-wrapper',
  templateUrl: './package-chart-wrapper.component.html',
  animations: [appModuleAnimation()],
})
export class PackageChartWrapperComponent implements OnInit {
  @Input() packageId: number;
  @Input() triggerReload: BehaviorSubject<boolean>;
  @Input() numHours: number = 24;
  @Input() triggerRender: BehaviorSubject<boolean>;

  kpisUpdate: KPIDto[];
  kpisChange: KPIDto[];

  constructor(private _packageService: PackageServiceProxy) {}

  ngOnInit(): void {
    this.loadKpis();

    if (this.triggerReload) {
      this.triggerReload.subscribe(val => {
        if (val) {
          this.loadKpis();
        }
      });
    }
  }

  loadKpis() {
    this.resetKpis();
    this.loadChangeKpis();
    this.loadUpdateKpis();
  }

  resetKpis() {
    this.kpisUpdate = undefined;
    this.kpisChange = undefined;
  }

  addKpis(data: KPIDto[], isChange: boolean = false) {
    if (isChange) {
      if (!this.kpisChange) {
        this.kpisChange = data;
        return;
      }
      this.kpisChange = this.kpisChange.concat(data);
    } else {
      if (!this.kpisUpdate) {
        this.kpisUpdate = data;
        return;
      }
      this.kpisUpdate = this.kpisUpdate.concat(data);
    }
  }

  loadUpdateKpis() {
    this._packageService
      .getStatusUpdateKPIs(this.packageId, this.numHours)
      .subscribe((data: KPIDto[]) => {
        this.addKpis(data);
      });
  }

  loadChangeKpis() {
    this._packageService
      .getStatusChangeKPIs(this.packageId, this.numHours)
      .subscribe((data: KPIDto[]) => {
        this.addKpis(data, true);
      });
  }
}
