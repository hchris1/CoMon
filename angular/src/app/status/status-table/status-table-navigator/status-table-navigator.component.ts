import {Component, EventEmitter, Input, Output} from '@angular/core';
import {
  StatusPreviewDto,
  StatusPreviewDtoPagedResultDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-status-table-navigator',
  templateUrl: './status-table-navigator.component.html',
})
export class StatusTableNavigatorComponent {
  @Input() isLoading: boolean = true;
  @Input() statusPreviews: StatusPreviewDtoPagedResultDto;
  @Input() hasMoreStatuses: boolean;
  @Input() statusIdToHighlight: number;
  @Input() openStatusModalOnClick: boolean = true;
  @Output() moreStatusesRequested = new EventEmitter();
  @Output() statusClicked = new EventEmitter<StatusPreviewDto>();

  onStatusClicked(status: StatusPreviewDto) {
    this.statusClicked.emit(status);
  }
}
