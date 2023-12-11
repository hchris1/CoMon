import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, TemplateRef } from '@angular/core';
import { CoMonHubService } from '@app/comon-hub.service';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { DynamicStylesHelper } from '@shared/helpers/DynamicStylesHelper';
import { GroupPreviewDto, GroupServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-group-summary',
  templateUrl: './group-summary.component.html',
  animations: [appModuleAnimation()]
})
export class GroupSummaryComponent implements OnInit, OnDestroy {
  @Input() groupId: number;
  @Input() editMode: boolean = false;
  @Output() groupClicked = new EventEmitter<GroupPreviewDto>();
  @Output() groupDeleted = new EventEmitter<null>();

  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;
  group: GroupPreviewDto;

  confirmDeletionModal: BsModalRef;

  constructor(
    private _groupService: GroupServiceProxy,
    private _modalService: BsModalService,
    private _comonHubService: CoMonHubService
  ) { }

  ngOnInit(): void {
    this.statusChangeSubscription = this._comonHubService.statusUpdate.subscribe((update) => {
      if (update.groupIds.includes(this.groupId)) {
        this.loadGroup();
      }
    });

    this.connectionEstablishedSubscription = this._comonHubService.connectionEstablished.subscribe((established) => {
      if (established)
        this.loadGroup();
    });
  }

  loadGroup() {
    this._groupService.getPreview(this.groupId).subscribe((group) => {
      this.group = group;
    });
  }

  onGroupClick(group: GroupPreviewDto) {
    if (this.editMode)
      return;
    this.groupClicked.emit(group);
  }

  onDeleteClicked(template: TemplateRef<any>) {
    this.confirmDeletionModal = this._modalService.show(template, { class: 'modal-sm' });
  }

  confirmDeletion() {
    this._groupService.delete(this.group.id).subscribe(() => {
      this.groupDeleted.emit();
      this.confirmDeletionModal.hide();
    });
  }

  declineDeletion() {
    this.confirmDeletionModal.hide();
  }

  getCardOutlineClass() {
    return DynamicStylesHelper.getCardOutlineClass(this.group.worstStatus.criticality);
  }

  getEmoji() {
    return DynamicStylesHelper.getEmoji(this.group.worstStatus.criticality);
  }

  ngOnDestroy(): void {
    this.statusChangeSubscription.unsubscribe();
    this.connectionEstablishedSubscription.unsubscribe();
  }
}
