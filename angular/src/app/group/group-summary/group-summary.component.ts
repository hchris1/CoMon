import { Component, EventEmitter, Input, Output, TemplateRef } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { GroupDto, GroupServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-group-summary',
  templateUrl: './group-summary.component.html',
  animations: [appModuleAnimation()]
})
export class GroupSummaryComponent {
  @Input() group: GroupDto;
  @Input() editMode: boolean = false;
  @Output() groupClicked = new EventEmitter<GroupDto>();
  @Output() groupDeleted = new EventEmitter<null>();

  confirmDeletionModal: BsModalRef;

  constructor(
    private _groupService: GroupServiceProxy,
    private _modalService: BsModalService
  ) { }

  onGroupClick(group: GroupDto) {
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
}