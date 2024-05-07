using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Inventory_Management_System.Helpers;
using Inventory_Management_System.Interfaces;
using Microsoft.Extensions.Options;

namespace Inventory_Management_System.Service
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _Cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );
            _Cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _Cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Url == null)
            {
                Uri defaultImageUrl = new(GetDefaultProfilePictureUrl());
                uploadResult.Url = defaultImageUrl;
                uploadResult.PublicId = null; // Set public ID to null for default image
            }

            return uploadResult;
        }

        private string GetDefaultProfilePictureUrl()
        {

            var transformation = new Transformation()
                .Width(150) // Adjust the width as per your requirement
                .Height(150) // Adjust the height as per your requirement
                .Crop("fill")
                .Gravity("auto")
                .Radius("max")
                .Effect("sharpen", 100); // Apply any desired effects

            var defaultImageUrl = _Cloudinary.Api.UrlImgUp.Transform(transformation)
                                      .BuildUrl("https://cdn3.iconfinder.com/data/icons/bold-ui-2/24/artsebasov_profile-512.png");

            return defaultImageUrl;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _Cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}
