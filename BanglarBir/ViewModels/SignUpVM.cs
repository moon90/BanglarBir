using BanglarBir.Filter;
using System.ComponentModel.DataAnnotations;

namespace BanglarBir.ViewModels
{
    public class SignUpVM
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "NIdOrStudentId is required")]
        public string NIdOrStudentId { get; set; }

        [Required(ErrorMessage = "Email or Phone is required")]
        [EmailOrPhone]
        public string EmailOrPhone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Url(ErrorMessage = "Invalid URL")]
        public string? FbProfileUrl { get; set; }

        public string? StudentIdOrNidPhoto { get; set; } = string.Empty;
    }
}
