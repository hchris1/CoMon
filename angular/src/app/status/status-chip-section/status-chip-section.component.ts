import {Component, Input, OnInit} from '@angular/core';
import {StatusDto, TriggerCause} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-status-chip-section',
  templateUrl: './status-chip-section.component.html',
})
export class StatusChipSectionComponent implements OnInit {
  @Input() status: StatusDto;

  chips: Chip[] = [];

  triggerCauseBadgeClass = 'badge-info';

  ngOnInit(): void {
    if (!this.status) return;

    switch (this.status.triggerCause) {
      case TriggerCause._0: // Unknown
        this.chips.push({
          text: 'Status.TriggerCause.Unknown',
          badgeClass: this.triggerCauseBadgeClass,
          tooltip: 'Status.TriggerCause.Unknown.Tooltip',
          icon: 'fa-question',
        });
        break;
      case TriggerCause._1: // Initialized
        this.chips.push({
          text: 'Status.TriggerCause.Initialized',
          badgeClass: this.triggerCauseBadgeClass,
          tooltip: 'Status.TriggerCause.Initialized.Tooltip',
          icon: 'fa-rocket',
        });
        break;

      case TriggerCause._2: // Scheduled
        this.chips.push({
          text: 'Status.TriggerCause.Scheduled',
          badgeClass: this.triggerCauseBadgeClass,
          tooltip: 'Status.TriggerCause.Scheduled.Tooltip',
          icon: 'fa-clock',
        });
        break;

      case TriggerCause._3: // Manual
        this.chips.push({
          text: 'Status.TriggerCause.Manual',
          badgeClass: this.triggerCauseBadgeClass,
          tooltip: 'Status.TriggerCause.Manual.Tooltip',
          icon: 'fa-hand-pointer',
        });
        break;

      case TriggerCause._4: // External
        this.chips.push({
          text: 'Status.TriggerCause.External',
          badgeClass: this.triggerCauseBadgeClass,
          tooltip: 'Status.TriggerCause.External.Tooltip',
          icon: 'fa-external-link',
        });
        break;
    }
  }
}

interface Chip {
  text: string;
  badgeClass: string;
  tooltip: string;
  icon: string;
}
