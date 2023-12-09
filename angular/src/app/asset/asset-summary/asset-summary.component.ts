import { ChangeDetectorRef, Component, EventEmitter, Input, OnDestroy, OnInit, Output, TemplateRef } from '@angular/core';
import { CoMonHubService } from '@app/comon-hub.service';
import { DynamicStylesHelper } from '@shared/helpers/DynamicStylesHelper';
import { AssetDto, AssetServiceProxy, PackageDto } from '@shared/service-proxies/service-proxies';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-asset-summary',
  templateUrl: './asset-summary.component.html',
})
export class AssetSummaryComponent implements OnInit, OnDestroy {
  @Input() assetId: number;
  @Input() editMode: boolean = false;
  @Output() assetClicked = new EventEmitter<AssetDto>();
  @Output() assetDeleted = new EventEmitter<null>();

  asset: AssetDto;

  confirmDeletionModal: BsModalRef;
  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;

  constructor(
    private _comonHubService: CoMonHubService,
    private _assetService: AssetServiceProxy,
    private _changeDetector: ChangeDetectorRef,
    private _modalService: BsModalService
  ) { }

  ngOnInit(): void {
    this.statusChangeSubscription = this._comonHubService.statusUpdate.subscribe((status) => {
      if (this.assetId === status.package.asset.id) {
        this.loadAsset();
      }
    });

    this.connectionEstablishedSubscription = this._comonHubService.connectionEstablished.subscribe((established) => {
      if (established)
        this.loadAsset();
    });
  }

  loadAsset() {
    this._assetService.get(this.assetId).subscribe((asset) => {
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
    if (this.editMode)
      return;
    this.assetClicked.emit(asset);
  }

  getEmoji() {
    const worstCriticality = DynamicStylesHelper.getWorstCriticality(this.asset);
    return DynamicStylesHelper.getEmoji(worstCriticality);
  }

  openDeletionModal(template: TemplateRef<any>) {
    this.confirmDeletionModal = this._modalService.show(template, { class: 'modal-sm' });
  }

  confirmDeletion() {
    this._assetService.delete(this.asset.id).subscribe(() => {
      this.assetDeleted.emit();
      this.confirmDeletionModal.hide();
    });
  }

  cancelDeletion() {
    this.confirmDeletionModal.hide();
  }
}
