using System.ComponentModel.DataAnnotations;

namespace ITSAssignment.Web.Models
{
    public class AdminAddMumineenViewModel
    {
        [Required]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "ITS must be 8 digits.")]
        public int Its { get; set; }

        [Required]
        [StringLength(75, ErrorMessage = "Name can contain maximum 75 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
