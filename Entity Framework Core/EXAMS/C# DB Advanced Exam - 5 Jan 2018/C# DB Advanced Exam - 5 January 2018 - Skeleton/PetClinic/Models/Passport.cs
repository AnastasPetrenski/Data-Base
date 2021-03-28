using System;
using System.ComponentModel.DataAnnotations;

namespace PetClinic.Models
{
    public class Passport
    {
        [Key]
        public string SerialNumber { get; set; }

        public int AnimalId { get; set; }
        [Required]
        public virtual Animal Animal { get; set; }

        [Required]
        public string OwnerPhoneNumber { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string OwnerName { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }
    }
}