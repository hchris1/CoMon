using Abp.Domain.Entities;
using Abp.MimeTypes;
using Abp.Runtime.Validation;
using CoMon.Assets;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace CoMon.Images
{
    [Table("CoMonImages")]
    public class Image : Entity<long>
    {
        public const int MaxBytes = 2 * 1024 * 1024;

        [MaxLength(MaxBytes)]
        public byte[] Data { get; set; }
        public string MimeType { get; set; }


        [ForeignKey(nameof(Asset))]
        public long AssetId { get; set; }
        public Asset Asset { get; set; }

        public static Image CreateImageFromFormFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                throw new AbpValidationException("Invalid file.");

            if (file.Length > MaxBytes)
                throw new AbpValidationException("File size larger than 2MB.");

            // Check if mime type is valid
            var mimeType = file.ContentType;
            if (mimeType != "image/jpeg" && mimeType != "image/png" && mimeType != "image/gif" && mimeType != "image/svg+xml")
                throw new AbpValidationException("Invalid mime type. Must be image/jpeg, image/png, image/gif, or image/svg.");

            using var stream = file.OpenReadStream();
            using var binaryReader = new BinaryReader(stream);
            var data = binaryReader.ReadBytes((int)file.Length);

            return new Image()
            {
                Data = data,
                MimeType = mimeType
            };
        }
    }
}
