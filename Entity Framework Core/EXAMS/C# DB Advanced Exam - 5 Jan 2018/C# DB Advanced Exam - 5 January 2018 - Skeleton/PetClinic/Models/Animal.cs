using PetClinic.DataProcessor.Dto.Import;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetClinic.Models
{
    public class Animal
    {
        public Animal()
        {
            this.Procedures = new HashSet<Procedure>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Type { get; set; }

        [Required]
        [Range(1, 130)]
        public int Age { get; set; }

        public string PassportSerialNumber { get; set; }
        [Required]
        public virtual Passport Passport { get; set; }

        public virtual ICollection<Procedure> Procedures { get; set; }

        public override bool Equals(object obj)
        {
            var animalDto = obj as JsonAnimalDto;

            if (this.PassportSerialNumber == animalDto.Passport.SerialNumber)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.Name);
            hash.Add(this.PassportSerialNumber);
            return hash.ToHashCode();
        }
    }
}

