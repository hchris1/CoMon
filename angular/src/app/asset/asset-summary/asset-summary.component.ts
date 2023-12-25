import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Injector,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import {CoMonHubService} from '@app/comon-hub.service';
import {AppComponentBase} from '@shared/app-component-base';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {
  AssetDto,
  AssetServiceProxy,
  PackageDto,
} from '@shared/service-proxies/service-proxies';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-asset-summary',
  templateUrl: './asset-summary.component.html',
})
export class AssetSummaryComponent
  extends AppComponentBase
  implements OnInit, OnDestroy
{
  @Input() assetId: number;
  @Input() editMode: boolean = false;
  @Input() showPath: boolean = false;
  @Input() showImage: boolean = true;
  @Input() showPackages: boolean = true;
  @Output() assetClicked = new EventEmitter<AssetDto>();
  @Output() assetDeleted = new EventEmitter();

  asset: AssetDto;

  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;

  constructor(
    private _comonHubService: CoMonHubService,
    private _assetService: AssetServiceProxy,
    private _changeDetector: ChangeDetectorRef,
    injector: Injector
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.statusChangeSubscription =
      this._comonHubService.statusUpdate.subscribe(update => {
        if (this.assetId === update.assetId) {
          this.loadAsset();
        }
      });

    this.connectionEstablishedSubscription =
      this._comonHubService.connectionEstablished.subscribe(established => {
        if (established) this.loadAsset();
      });
  }

  loadAsset() {
    this._assetService.get(this.assetId).subscribe(asset => {
      this.asset = asset;
      this._changeDetector.detectChanges();
    });
  }

  ngOnDestroy(): void {
    this.statusChangeSubscription.unsubscribe();
    this.connectionEstablishedSubscription.unsubscribe();
  }

  getTitleClass(asset: AssetDto) {
    const worstCriticality = DynamicStylesHelper.getWorstCriticality(asset);
    return DynamicStylesHelper.getBackgroundClass(worstCriticality);
  }

  getBadgeClass(pack: PackageDto) {
    return DynamicStylesHelper.getBadgeClass(pack.lastCriticality);
  }

  getCardOutlineClass(asset: AssetDto) {
    const worstCriticality = DynamicStylesHelper.getWorstCriticality(asset);
    return DynamicStylesHelper.getCardOutlineClass(worstCriticality);
  }

  onAssetClick(asset: AssetDto) {
    if (this.editMode) return;
    this.assetClicked.emit(asset);
  }

  getEmoji() {
    const worstCriticality = DynamicStylesHelper.getWorstCriticality(
      this.asset
    );
    return DynamicStylesHelper.getEmoji(worstCriticality);
  }

  deleteAssetClicked() {
    this.message.confirm(
      this.l('Assets.DeleteConfirmationMessage', this.asset.name),
      this.l('Assets.DeleteConfirmationTitle'),
      isConfirmed => {
        if (isConfirmed) {
          this._assetService.delete(this.asset.id).subscribe(() => {
            this.assetDeleted.emit();
            this.notify.success(
              this.l('Assets.DeleteSuccessMessage'),
              this.asset.name
            );
          });
        }
      }
    );
  }
}
