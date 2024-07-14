using System.ComponentModel.DataAnnotations;

namespace CoMon.Assets
{
    public class CreateAssetDto
    {
        [Required]
        [StringLength(Asset.MaxNameLength, MinimumLength = 1)]
        public string Name { get; set; }
        public string Description { get; set; }
        public long? GroupId { get; set; }
    }
}