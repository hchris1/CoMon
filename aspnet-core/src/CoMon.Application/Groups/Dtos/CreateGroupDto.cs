using System.ComponentModel.DataAnnotations;

namespace CoMon.Groups
{
    public class CreateGroupDto
    {
        [Required]
        [StringLength(256, MinimumLength = 1)]
        public string Name { get; set; }
        public long? ParentId { get; set; }
    }
}