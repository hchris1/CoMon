import {Component, Input, OnInit} from '@angular/core';
import {StatusDto, TriggerCause} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-status-chip',
  templateUrl: './status-chip.component.html',
})
export class StatusChipSectionComponent implements OnInit {
  @Input() status: StatusDto;

  chip: Chip;

  triggerCauseBadgeClass = 'badge-info';

  ngOnInit(): void {
    if (!this.status) return;

    switch (this.status.triggerCause) {
      case TriggerCause._0: // Unknown
        this.chip = {
          tooltip: 'Status.TriggerCause.Unknown.Tooltip',
          icon: 'fa-question',
        };
        break;
      case TriggerCause._1: // Initialized
        this.chip = {
          tooltip: 'Status.TriggerCause.Initialized.Tooltip',
          icon: 'fa-rocket',
        };
        break;

      case TriggerCause._2: // Scheduled
        this.chip = {
          tooltip: 'Status.TriggerCause.Scheduled.Tooltip',
          icon: 'fa-clock',
        };
        break;

      case TriggerCause._3: // Manual
        this.chip = {
          tooltip: 'Status.TriggerCause.Manual.Tooltip',
          icon: 'fa-hand-pointer',
        };
        break;

      case TriggerCause._4: // External
        this.chip = {
          tooltip: 'Status.TriggerCause.External.Tooltip',
          icon: 'fa-external-link',
        };
        break;
    }
  }
}

interface Chip {
  tooltip: string;
  icon: string;
}
