import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import {
  StatusHistoryDto,
  StatusPreviewDto,
  StatusServiceProxy,
} from '@shared/service-proxies/service-proxies';
import {BehaviorSubject} from 'rxjs';

@Component({
  selector: 'app-status-timeline',
  templateUrl: './status-timeline.component.html',
})
export class StatusTimelineComponent implements OnInit, OnDestroy {
  @Input() status: StatusPreviewDto;
  @Input() reloadHistory: BehaviorSubject<StatusPreviewDto>;
  @Output() statusClicked = new EventEmitter<StatusPreviewDto>();

  statusHistory: StatusHistoryDto;

  constructor(
    private _statusService: StatusServiceProxy,
    private _changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadHistory(this.status.id);

    this.reloadHistory.subscribe((statusPreview: StatusPreviewDto) => {
      this.loadHistory(statusPreview.id);
    });
  }

  loadHistory(id: number) {
    this._statusService.getHistory(id).subscribe(result => {
      this.statusHistory = result;
      this._changeDetector.detectChanges();
    });
  }

  ngOnDestroy() {
    this.reloadHistory.unsubscribe();
  }
}
