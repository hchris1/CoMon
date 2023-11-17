using Abp.Domain.Entities;
using CoMon.Assets;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Images
{
    [Table("CoMonImages")]
    public class Image : Entity<long>
    {
        public byte[] Data { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }

        [ForeignKey(nameof(Asset))]
        public long AssetId { get; set; }
        public Asset Asset { get; set; }
    }
}
