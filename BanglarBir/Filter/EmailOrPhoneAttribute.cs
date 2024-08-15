using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BanglarBir.Filter
{
    public class EmailOrPhoneAttribute : ValidationAttribute
    {
        public EmailOrPhoneAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("Email or Phone is required");
            }

            var input = value.ToString();
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            var phonePattern = @"^\+?8801[3-9]\d{8}$|^01[3-9]\d{8}$"; // Bangladeshi phone numbers

            if (Regex.IsMatch(input, emailPattern) || Regex.IsMatch(input, phonePattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid email address or phone number");
        }
    }
}
