import {
  Component,
  Injector,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import {AppComponentBase} from '@shared/app-component-base';
import {
  DashboardServiceProxy,
  DashboardTileDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-markdown-tile',
  templateUrl: './markdown-tile.component.html',
})
export class MarkdownTileComponent
  extends AppComponentBase
  implements OnInit, OnChanges
{
  @Input() dashboardId: number;
  @Input() tile: DashboardTileDto;
  @Input() editMode: boolean = false;

  initialized = false;
  markdown: string;

  constructor(
    injector: Injector,
    private _dashboardService: DashboardServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.markdown = this.tile.content;
    this.initialized = true;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (!this.initialized) return;

    if (!!changes.editMode && changes.editMode.currentValue === false) {
      if (this.markdown !== this.tile.content) {
        this.tile.content = this.markdown;
        this.saveMarkdown();
      }
    }
  }

  saveMarkdown() {
    this._dashboardService
      .updateTileContent(this.dashboardId, this.tile.id, this.markdown)
      .subscribe();
  }
}
