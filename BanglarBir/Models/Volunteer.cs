using BanglarBir.Filter;
using System.ComponentModel.DataAnnotations;

namespace BanglarBir.Models
{
    public class Volunteer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "NIdOrStudentId is required")]
        public string NIdOrStudentId { get; set; }

        [Required(ErrorMessage = "EmailOrPhone is required")]
        [EmailOrPhone]
        public string EmailOrPhone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Location { get; set; }
        public string? FbProfileUrl { get; set; }
        public string? StudentIdOrNidPhoto { get; set; }
        // Add a Role property
        public string Role { get; set; }
    }
}
