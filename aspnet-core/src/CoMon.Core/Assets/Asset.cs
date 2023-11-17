using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using CoMon.Groups;
using CoMon.Images;
using CoMon.Packages;

namespace CoMon.Assets
{
    [Table("CoMonAssets")]
    public class Asset : Entity<long>
    {
        public const int MaxNameLength = 256;
        public const int MaxDescriptionLength = 2048;

        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }
        [StringLength(MaxDescriptionLength)]
        public virtual string Description { get; set; }
        public virtual List<Image> Images { get; set; } = new List<Image>();
        public virtual List<Package> Packages { get; set; }
        public virtual Group Group { get; set; }
    }
}
