using System.ComponentModel.DataAnnotations;

namespace CoMon.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}