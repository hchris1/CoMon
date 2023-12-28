import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
} from '@angular/core';
import {
  StatusHistoryDto,
  StatusPreviewDto,
  StatusServiceProxy,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-status-timeline',
  templateUrl: './status-timeline.component.html',
})
export class StatusTimelineComponent implements OnChanges {
  @Input() statusId: number;
  @Input() reloadHistory: EventEmitter<boolean>;
  @Output() statusClicked = new EventEmitter<StatusPreviewDto>();

  statusHistory: StatusHistoryDto;

  constructor(
    private _statusService: StatusServiceProxy,
    private _changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.reloadHistory.subscribe(() => {
      this.loadHistory(this.statusId);
    });
  }

  ngOnChanges() {
    this.loadHistory(this.statusId);
  }

  loadHistory(id: number) {
    this.statusHistory = undefined;
    this._statusService.getHistory(id).subscribe(result => {
      this.statusHistory = result;
      this._changeDetector.detectChanges();
    });
  }
}
