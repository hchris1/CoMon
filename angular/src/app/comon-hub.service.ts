import { EventEmitter, Injectable, Output } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { AppConsts } from '@shared/AppConsts';
import { Criticality, StatusDto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { Moment } from 'moment';
import { ActiveToast, ToastrService } from 'ngx-toastr';
import { DynamicStylesHelper } from '@shared/helpers/DynamicStylesHelper';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { StatusModalComponent } from './status/status-modal/status-modal.component';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CoMonHubService {
  private hubConnection: HubConnection;

  statusModalRef: BsModalRef;

  @Output() statusUpdate: EventEmitter<StatusDto> = new EventEmitter<StatusDto>();
  @Output() connectionEstablished: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(
    private _toastrService: ToastrService,
    private _modalService: BsModalService
  ) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(AppConsts.remoteServiceBaseUrl + '/signalr-comonhub')
      .build();

    this.startConnection();

    this.hubConnection.onclose(() => {
      console.log('Connection closed. Retrying...');
      this.connectionEstablished.next(false);
      setTimeout(() => {
        this.startConnection();
      }, 5000);
    });

    this.hubConnection.on('CoMon.Status.Update', (json: string) => {
      const status = JSON.parse(json);
      this.statusUpdate.emit(status);
    });

    this.hubConnection.on('CoMon.Status.Change', (json: string) => {
      const update = JSON.parse(json);
      this.createStatusChangeToast(update);
    });
  }

  startConnection() {
    console.log('Starting connection');
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.connectionEstablished.next(true);
      })
      .catch(() => {
        console.log('Error while establishing connection. Retrying...');
        setTimeout(() => {
          this.startConnection();
        }, 5000);
      });
  }

  createStatusChangeToast(update) {
    const criticality: Criticality = update.criticality;
    const previousCriticality: Criticality = update.previousCriticality;
    const time: Moment = moment(update.time);

    const title = update.assetName + ': ' + update.packageName;
    const message = DynamicStylesHelper.getEmoji(previousCriticality) + '→' + DynamicStylesHelper.getEmoji(criticality) + ' ' + time.format('HH:mm:ss');

    let toast: ActiveToast<any>;
    switch (criticality) {
      case Criticality._1:
        toast = this._toastrService.success(message, title);
        break;
      case Criticality._3:
        toast = this._toastrService.warning(message, title);
        break;
      case Criticality._5:
        toast = this._toastrService.error(message, title);
        break;
      default:
        toast = this._toastrService.info(message, title);
        break;
    }

    toast.onTap.subscribe(() => {
      this.openStatusModal(update.id);
    });

    // TODO: Push notifications
    // Push.create(title, {
    //   body: message,
    //   icon: abp.appPath + 'assets/img/logo.svg',
    //   timeout: 6000,
    //   onClick: function () {
    //     window.focus();
    //     this.close();
    //   }
    // });
  }

  openStatusModal(statusId: number) {

    // Add this to src/styles.scss:



    this.statusModalRef = this._modalService.show(StatusModalComponent,
      {
        class: 'status-modal',
        initialState: { statusId: statusId },
      }
    );
    this.statusModalRef.content.closeBtnName = 'Close';

    this.statusModalRef.content.onClose.subscribe(() => {
      this.statusModalRef.hide();
    });
  }
}
