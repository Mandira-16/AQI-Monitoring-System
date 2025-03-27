using System.ComponentModel.DataAnnotations;

namespace AQI_Monitoring_System.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Username must be at least 8 characters long")]
        public string Username { get; set; } // Add ? for nullable

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } // Add ? for nullable

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).{6,}$",
            ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one special character")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } // Add ? for nullable

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } // Add ? for nullable
    }
}