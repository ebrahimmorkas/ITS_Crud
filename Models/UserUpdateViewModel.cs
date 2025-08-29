using System.ComponentModel.DataAnnotations;

namespace ITSAssignment.Web.Models
{
    public class UserUpdateViewModel
    {
        [Required]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "ITS must be 8 digits.")]
        public int Its { get; set; }

        [Required]
        [StringLength(75, ErrorMessage = "Name can contain maximum 75 characters.")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Age must be between 1 and 100.")]
        public int Age { get; set; }

        [Required]
        [RegularExpression(@"^(male|female)$", ErrorMessage = "Gender must be either male or female.")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Enter a valid mobile number with country code.")]
        public string Mobile_number { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email_address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(Single|Engaged|Married|Divorced|Widowed)$", ErrorMessage = "Invalid marital status.")]
        public string Marital_status { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = string.Empty;
    }
}
