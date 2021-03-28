using System.ComponentModel.DataAnnotations;

namespace PetClinic.DataProcessor.Dto.Import
{
    public class JsonAnimalDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Type { get; set; }

        [Required]
        [Range(1, 130)]
        public int Age { get; set; }

        [Required]
        public JsonPassportDto Passport { get; set; }
    }
}
