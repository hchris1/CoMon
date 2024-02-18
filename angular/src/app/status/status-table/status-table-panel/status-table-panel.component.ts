import {Component, Input} from '@angular/core';
import {AssistantModalComponent} from '@app/common/assistant-modal/assistant-modal.component';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {StatusDto} from '@shared/service-proxies/service-proxies';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-status-table-panel',
  templateUrl: './status-table-panel.component.html',
})
export class StatusTablePanelComponent {
  @Input() status: StatusDto;
  @Input() isLoading: boolean;

  assistantModalRef: BsModalRef;

  constructor(private _modalService: BsModalService) {}

  getBackgroundStyle() {
    return DynamicStylesHelper.getHistoricBackgroundClass(this.status.isLatest);
  }

  openAssistant() {
    this.assistantModalRef = this._modalService.show(AssistantModalComponent, {
      initialState: {
        statusId: this.status.id,
      },
      class: 'modal-lg',
    });
    this.assistantModalRef.content.closeBtnName = 'Close';

    this.assistantModalRef.content.onClose.subscribe(() => {
      this.assistantModalRef.hide();
    });
  }
}
