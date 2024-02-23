using Abp.Domain.Entities;
using CoMon.Assets;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoMon.Groups
{
    [Table("CoMonGroups")]
    public class Group : Entity<long>
    {
        public const int MaxNameLength = 256;

        [Required]
        [StringLength(MaxNameLength, MinimumLength = 1)]
        public string Name { get; set; }
        public List<Asset> Assets { get; set; } = [];
        public List<Group> SubGroups { get; set; } = [];

        [ForeignKey(nameof(Group))]
        public virtual long? ParentId { get; set; }
        public virtual Group Parent { get; set; }
    }
}
