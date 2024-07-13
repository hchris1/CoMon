using Abp.AutoMapper;

namespace CoMon.Images
{
    [AutoMapFrom(typeof(Image))]
    public class ImageCacheItem
    {
        public long Id { get; set; }
        public byte[] Data { get; set; }
        public string MimeType { get; set; }
    }
}
