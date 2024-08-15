using System.ComponentModel.DataAnnotations;

namespace BanglarBir.Models
{
    public class Victim
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Display(Name = "Profile Picture")]
        public string? Picture { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\+?8801[3-9]\d{8}$|^01[3-9]\d{8}$",ErrorMessage = "Phone is required and must be properly formatted.")]
        public string Phone { get; set; }

        [Display(Name = "bKash Number")]
        [RegularExpression(@"^\+?8801[3-9]\d{8}$|^01[3-9]\d{8}$", ErrorMessage = "Phone is required and must be properly formatted.")]
        public string? bKashNumber { get; set; }

        [Required(ErrorMessage = "Donation Needed status is required")]
        [Display(Name = "Donation Needed")]
        public bool DonationNeeded { get; set; }

        public decimal? DonationConfirmed { get; set; } = 0;

        [Required(ErrorMessage = "Location is required")]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Is Student")]
        [Required(ErrorMessage = "Is Student status is required")]
        public bool IsStudent { get; set; }
        public int VolunteerId { get; set; }
    }
}
