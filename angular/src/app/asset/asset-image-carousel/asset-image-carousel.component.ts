import { Component, Input, OnInit } from '@angular/core';
import { AssetServiceProxy, FileParameter, ImageDto, ImageServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-asset-image-carousel',
  templateUrl: './asset-image-carousel.component.html'
})
export class AssetImageCarouselComponent implements OnInit {
  @Input() assetId: number;
  @Input() titleImageOnly: boolean = false;
  @Input() editMode: boolean = false;

  images: ImageDto[] = [];
  isLoading: boolean = false;

  constructor(
    private _assetService: AssetServiceProxy,
    private _imageService: ImageServiceProxy
  ) { }

  ngOnInit(): void {
    this.loadImages();
  }

  loadImages() {
    this.isLoading = true;
    if (this.titleImageOnly) {
      this._imageService.getTitleImageForAsset(this.assetId).subscribe((result) => {
        this.images = result.id !== undefined ? [result] : [];
        this.isLoading = false;
      });
    } else {
      this._imageService.getImagesForAsset(this.assetId).subscribe((result) => {
        this.images = result;
        this.isLoading = false;
      });
    }
  }

  addImage(image: FileParameter) {
    this._assetService.uploadImage(this.assetId, image).subscribe(() => {
      this.loadImages();
    });
  }

  deleteImage(image: ImageDto) {
    this._imageService.delete(image.id).subscribe(() => {
      this.images = this.images.filter(x => x.id != image.id);
    });
  }
}
