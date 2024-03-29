import {appModuleAnimation} from '@shared/animations/routerTransition';
import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import {
  StatusPreviewDto,
  StatusServiceProxy,
} from '@shared/service-proxies/service-proxies';
import {CoMonHubService} from '@app/comon-hub.service';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-package-preview',
  templateUrl: './package-preview.component.html',
  animations: [appModuleAnimation()],
})
export class PackagePreviewComponent implements OnInit, OnDestroy {
  @Input() packageId: number;
  @Input() editMode: boolean = false;
  @Input() showPath: boolean = false;
  @Input() showDate: boolean = true;
  @Input() showTimeline: boolean = false;
  @Output() packageDeleted = new EventEmitter();
  @Output() packageEdited = new EventEmitter();

  status: StatusPreviewDto;
  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;

  constructor(
    private _statusService: StatusServiceProxy,
    private _comonHubService: CoMonHubService
  ) {}

  ngOnInit() {
    this.statusChangeSubscription =
      this._comonHubService.statusUpdate.subscribe(update => {
        if (this.packageId === update.packageId) {
          this.loadStatus();
        }
      });

    this.connectionEstablishedSubscription =
      this._comonHubService.connectionEstablished.subscribe(established => {
        if (established) this.loadStatus();
      });
  }

  loadStatus() {
    this._statusService
      .getLatestStatusPreview(this.packageId)
      .subscribe(status => {
        this.status = status;
      });
  }

  ngOnDestroy() {
    this.statusChangeSubscription.unsubscribe();
    this.connectionEstablishedSubscription.unsubscribe();
  }
}
