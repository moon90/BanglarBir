using BanglarBir.Filter;
using System.ComponentModel.DataAnnotations;

namespace BanglarBir.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email or Phone is required")]
        [EmailOrPhone]
        public string EmailOrPhone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
