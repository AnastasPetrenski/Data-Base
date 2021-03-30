using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicHub.DataProcessor.ImportDtos
{
    public class JsonProducerDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [RegularExpression(@"^[A-Z][a-z]+ [A-Z][a-z]+")]
        public string Pseudonym { get; set; }

        [RegularExpression(@"^\+359 \d{3} \d{3} \d{3}$")]
        public string PhoneNumber { get; set; }

        public ICollection<JsonAlbumDto> Albums { get; set; }
    }
}
