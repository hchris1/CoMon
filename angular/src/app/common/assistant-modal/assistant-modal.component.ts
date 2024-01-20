import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {AssistantServiceProxy} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-assistant-modal',
  templateUrl: './assistant-modal.component.html',
  animations: [appModuleAnimation()],
})
export class AssistantModalComponent implements OnInit {
  @Input() isRoot: boolean = false;
  @Input() groupId: number;
  @Input() assetId: number;
  @Input() statusId: number;
  @Output() onClose = new EventEmitter();

  response: string;

  constructor(private _assistantService: AssistantServiceProxy) {}

  ngOnInit() {
    if (this.isRoot) {
      this._assistantService.getGroupSummary(undefined).subscribe(result => {
        this.response = result;
        this.processResponse();
      });
    } else if (this.groupId) {
      this._assistantService.getGroupSummary(this.groupId).subscribe(result => {
        this.response = result;
        this.processResponse();
      });
    } else if (this.assetId) {
      this._assistantService.getAssetSummary(this.assetId).subscribe(result => {
        this.response = result;
        this.processResponse();
      });
    } else if (this.statusId) {
      this._assistantService
        .getRecommendations(this.statusId)
        .subscribe(result => {
          this.response = result;
          this.processResponse();
        });
    }
  }

  processResponse() {
    this.response = this.response.replace(/```html/g, '');
    this.response = this.response.replace(/```/g, '');
  }

  onCloseClicked() {
    this.onClose.emit();
  }
}
