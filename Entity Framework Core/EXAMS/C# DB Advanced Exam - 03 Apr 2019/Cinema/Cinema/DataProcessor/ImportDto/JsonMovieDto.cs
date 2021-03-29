using System.ComponentModel.DataAnnotations;

namespace Cinema.DataProcessor.ImportDto
{
    public class JsonMovieDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public string Duration { get; set; }

        [Required]
        [Range(1, 10)]
        public double Rating { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Director { get; set; }
    }
}
