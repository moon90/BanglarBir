using System.ComponentModel.DataAnnotations;

namespace BanglarBir.ViewModels
{
    public class VictimVM
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
        public string Phone { get; set; }

        [Display(Name = "bKash Number")]
        public string? bKashNumber { get; set; }

        [Required(ErrorMessage = "Donation Needed status is required")]
        [Display(Name = "Donation Needed")]
        public string DonationNeeded { get; set; }

        public decimal? DonationConfirmed { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Is Student")]
        [Required(ErrorMessage = "Is Student status is required")]
        public string IsStudent { get; set; }
        public int VolunteerId { get; set; }
        public string VolunteerName { get; set; }
    }
}
