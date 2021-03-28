using System.ComponentModel.DataAnnotations;

namespace PetClinic.DataProcessor.Dto.Import
{
    public class JsonPassportDto
    {
        [Required]
        [RegularExpression(@"^[A-Za-z]{7}\d{3}$")]
        public string SerialNumber { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string OwnerName { get; set; }

        [Required]
        [RegularExpression(@"(^[\d]{10}$)|(^\+359[\d]{9}$)")]
        public string OwnerPhoneNumber { get; set; }

        [Required]
        [RegularExpression(@"^\d{2}-\d{2}-\d{4}$")]
        public string RegistrationDate { get; set; }
    }
}
