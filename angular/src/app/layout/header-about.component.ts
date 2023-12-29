import {Component, ChangeDetectionStrategy, Injector} from '@angular/core';
import {AppComponentBase} from '@shared/app-component-base';
import {Moment} from 'moment';

@Component({
  selector: 'header-about',
  templateUrl: './header-about.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderAboutComponent extends AppComponentBase {
  version: string;
  buildDate: Moment;

  constructor(injector: Injector) {
    super(injector);

    this.version = this.appSession.application.version;
    this.buildDate = this.appSession.application.releaseDate;
  }

  openAboutModal() {
    this.message.info(
      'Version ' +
        this.version +
        ' (' +
        this.buildDate.format('HH:mm DD.MM.YYYY') +
        ')',
      this.l('About') + ' CoMon'
    );
  }
}
