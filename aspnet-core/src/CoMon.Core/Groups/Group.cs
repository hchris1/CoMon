using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using CoMon.Assets;

namespace CoMon.Groups
{
    [Table("CoMonGroups")]
    public class Group : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public List<Asset> Assets { get; set; } = new();
        public List<Group> SubGroups { get; set; } = new();

        [ForeignKey(nameof(Group))]
        public virtual Group Parent { get; set; }
    }
}
