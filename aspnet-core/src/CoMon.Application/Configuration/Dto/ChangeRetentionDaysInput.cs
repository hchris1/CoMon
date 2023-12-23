using System.ComponentModel.DataAnnotations;

namespace CoMon.Configuration.Dto
{
    public class ChangeRetentionDaysInput
    {
        [Required]
        [Range(-1, int.MaxValue)]
        public int Days { get; set; }
    }
}
