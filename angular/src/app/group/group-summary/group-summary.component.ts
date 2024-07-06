import {
  Component,
  EventEmitter,
  Injector,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import {debounceTime, filter, Subscription} from 'rxjs';
import {CoMonHubService} from '@app/comon-hub.service';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {AppComponentBase} from '@shared/app-component-base';
import {DynamicStylesHelper} from '@shared/helpers/DynamicStylesHelper';
import {
  GroupPreviewDto,
  GroupServiceProxy,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-group-summary',
  templateUrl: './group-summary.component.html',
  animations: [appModuleAnimation()],
})
export class GroupSummaryComponent
  extends AppComponentBase
  implements OnInit, OnDestroy
{
  @Input() groupId: number;
  @Input() editMode: boolean = false;
  @Input() showPath: boolean = false;
  @Output() groupClicked = new EventEmitter<GroupPreviewDto>();
  @Output() groupDeleted = new EventEmitter();

  statusChangeSubscription: Subscription;
  connectionEstablishedSubscription: Subscription;
  group: GroupPreviewDto;

  constructor(
    private _groupService: GroupServiceProxy,
    private _comonHubService: CoMonHubService,
    injector: Injector
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.statusChangeSubscription = this._comonHubService.statusUpdate
      .pipe(
        filter(update => update.groupIds.includes(this.groupId)),
        // Prevent lots of requests when multiple packages are updated at the same time
        debounceTime(500)
      )
      .subscribe(() => {
        this.loadGroup();
      });

    this.connectionEstablishedSubscription =
      this._comonHubService.connectionEstablished.subscribe(established => {
        if (established) this.loadGroup();
      });
  }

  loadGroup() {
    this._groupService.getPreview(this.groupId).subscribe(group => {
      this.group = group;
    });
  }

  onGroupClick(group: GroupPreviewDto) {
    if (this.editMode) return;
    this.groupClicked.emit(group);
  }

  deleteGroupClicked() {
    this.message.confirm(
      this.l('Group.DeleteConfirmationMessage'),
      this.l('Group.DeleteConfirmationTitle'),
      isConfirmed => {
        if (isConfirmed) {
          this._groupService.delete(this.group.id).subscribe(() => {
            this.groupDeleted.emit();
            this.notify.success(
              this.l('Group.DeleteSuccessMessage'),
              this.group.name
            );
          });
        }
      }
    );
  }

  getCardOutlineClass() {
    return DynamicStylesHelper.getCardOutlineClass(
      this.group.worstStatus?.criticality
    );
  }

  ngOnDestroy(): void {
    this.statusChangeSubscription.unsubscribe();
    this.connectionEstablishedSubscription.unsubscribe();
  }
}
