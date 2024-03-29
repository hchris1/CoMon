﻿using Abp.Domain.Uow;
using CoMon.Assets;
using CoMon.Images.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CoMon.Tests.Images
{
    public class ImageAppService_Tests : CoMonTestBase
    {
        private readonly ImageAppService _imageAppService;

        public ImageAppService_Tests()
        {
            _imageAppService = Resolve<ImageAppService>();
        }

        [Fact]
        public async Task GetTitleImageForAsset_AssetHasImage_ReturnsImage()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddImage();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            ImageDto result;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                result = await _imageAppService.GetTitleImageForAsset(1);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetTitleImageForAsset_AssetHasNoImage_ReturnsNull()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            ImageDto result;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                result = await _imageAppService.GetTitleImageForAsset(1);
            }

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetImagesForAsset_AssetHasImages_ReturnsImages()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddImage();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            List<ImageDto> result;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                result = await _imageAppService.GetImagesForAsset(1);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetImagesForAsset_AssetHasNoImages_ReturnsEmptyList()
        {
            // Arrange
            var arrangedAsset = EntityFactory.CreateAsset();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            List<ImageDto> result;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                result = await _imageAppService.GetImagesForAsset(1);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Delete_ImageExists_DeletesImage()
        {
            // Arrange
            var arrangedAsset = EntityFactory
                .CreateAsset()
                .AddImage();
            UsingDbContext(context => context.Assets.Add(arrangedAsset));

            // Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                await _imageAppService.Delete(1);
                await uow.CompleteAsync();
            }

            // Assert
            UsingDbContext(context =>
            {
                Assert.Null(context.Images.FirstOrDefault(i => i.Id == 1));
            });
        }
    }
}
