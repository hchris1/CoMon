using Abp.AutoMapper;

namespace CoMon.Images.Dtos
{
    [AutoMapFrom(typeof(Image))]
    public class ImageDto
    {
        public long Id { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public byte[] Data { get; set; }
    }
}
