import {
  Component,
  EventEmitter,
  Injector,
  Input,
  OnChanges,
  OnInit,
  Output,
} from '@angular/core';
import {AppComponentBase} from '@shared/app-component-base';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {
  Criticality,
  StatusPreviewDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-status-timeline-button',
  templateUrl: './status-timeline-button.component.html',
})
export class StatusTimelineButtonComponent
  extends AppComponentBase
  implements OnInit, OnChanges
{
  @Input() type: ButtonType;
  @Input() statusPreview: StatusPreviewDto;

  @Output() clicked = new EventEmitter();

  buttonClass: string;
  disabled: boolean;
  icon: string;
  infoText: string;
  tooltipText: string;

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.getInfoText();
    this.getTooltipText();
    this.getIcon();

    this.initButton();
  }

  ngOnChanges(): void {
    this.initButton();
  }

  initButton() {
    if (!this.statusPreview) {
      this.buttonClass = DynamicStylesHelper.getButtonClass(undefined);
      this.disabled = true;
      return;
    }
    this.disabled = false;
    this.buildButtonClass(this.statusPreview.criticality);
  }

  buildButtonClass(criticality: Criticality) {
    this.buttonClass = DynamicStylesHelper.getButtonClass(criticality);
  }

  getIcon() {
    switch (this.type) {
      case ButtonType.Previous:
        this.icon = 'fa-chevron-left';
        break;
      case ButtonType.Next:
        this.icon = 'fa-chevron-right';
        break;
      case ButtonType.Latest:
        this.icon = 'fa-history';
        break;
    }
  }

  getInfoText() {
    switch (this.type) {
      case ButtonType.Previous:
        this.infoText = this.localization.localize(
          'Status.NoPreviousStatus',
          this.localizationSourceName
        );
        break;
      case ButtonType.Next:
        this.infoText = this.localization.localize(
          'Status.NoNextStatus',
          this.localizationSourceName
        );
        break;
      case ButtonType.Latest:
        this.infoText = this.localization.localize(
          'Status.NoLatestStatus',
          this.localizationSourceName
        );
        break;
    }
  }

  getTooltipText() {
    switch (this.type) {
      case ButtonType.Previous:
        this.tooltipText = this.localization.localize(
          'Status.PreviousStatusTooltip',
          this.localizationSourceName
        );
        break;
      case ButtonType.Next:
        this.tooltipText = this.localization.localize(
          'Status.NextStatusTooltip',
          this.localizationSourceName
        );
        break;
      case ButtonType.Latest:
        this.tooltipText = this.localization.localize(
          'Status.LatestStatusTooltip',
          this.localizationSourceName
        );
        break;
    }
  }
}

enum ButtonType {
  Previous = 0,
  Next = 1,
  Latest = 2,
}
