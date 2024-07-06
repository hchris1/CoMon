import {Component, Injector, Input} from '@angular/core';
import {AppComponentBase} from '@shared/app-component-base';
import {
  DashboardServiceProxy,
  DashboardTileDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-markdown-tile',
  templateUrl: './markdown-tile.component.html',
})
export class MarkdownTileComponent extends AppComponentBase {
  @Input() dashboardId: number;
  @Input() tile: DashboardTileDto;
  @Input() editMode: boolean = false;

  markdown: string;

  constructor(
    private _dashboardService: DashboardServiceProxy,
    injector: Injector
  ) {
    super(injector);
  }

  ngOnChanges(changes) {
    if (!this.markdown) {
      this.markdown = this.tile.content;
      return;
    }

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
