using BanglarBir.Filter;
using System.ComponentModel.DataAnnotations;

namespace BanglarBir.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Email or Phone is required")]
        [EmailOrPhone]
        public string EmailOrPhone { get; set; }

        //[Required(ErrorMessage = "New Password is required")]
        //[StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        //[DataType(DataType.Password)]
        //public string NewPassword { get; set; }

        //[Required(ErrorMessage = "Confirm Password is required")]
        //[DataType(DataType.Password)]
        //[Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match")]
        //public string ConfirmPassword { get; set; }
    }
}
