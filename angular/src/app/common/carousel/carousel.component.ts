import {
  Component,
  EventEmitter,
  Injector,
  Input,
  Output,
  TemplateRef,
} from '@angular/core';
import {AppComponentBase} from '@shared/app-component-base';
import {FileParameter, ImageDto} from '@shared/service-proxies/service-proxies';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-carousel',
  templateUrl: './carousel.component.html',
})
export class CarouselComponent extends AppComponentBase {
  @Input() images: ImageDto[];
  @Input() hideIndicators: boolean = false;
  @Input() editMode: boolean = false;

  @Output() imageAdded = new EventEmitter<FileParameter>();
  @Output() imageDeleted = new EventEmitter<ImageDto>();

  modalRef?: BsModalRef;
  imageToAdd?: FileParameter;

  constructor(
    private _modalService: BsModalService,
    injector: Injector
  ) {
    super(injector);
  }

  deleteImageClicked(image: ImageDto) {
    this.message.confirm(
      this.l('Image.DeleteConfirmationMessage'),
      this.l('Image.DeleteConfirmationTitle'),
      isConfirmed => {
        if (isConfirmed) {
          this.imageDeleted.emit(image);
        }
      }
    );
  }

  /* eslint-disable @typescript-eslint/no-explicit-any */
  addImageClicked(template: TemplateRef<any>) {
    this.modalRef = this._modalService.show(template, {class: 'modal-sm'});
  }

  onFileSelected(event) {
    const file: File = event.target.files[0];

    // Load the image
    const reader = new FileReader();
    reader.onload = e => {
      this.imageToAdd = {
        fileName: file.name,
        data: new Blob([e.target.result], {type: file.type}),
      };
    };
    reader.readAsArrayBuffer(file);
  }

  confirmAddImage() {
    this.imageAdded.emit(this.imageToAdd);
    this.modalRef?.hide();
  }

  declineAddImage() {
    this.modalRef?.hide();
  }
}
