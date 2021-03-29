using System.ComponentModel.DataAnnotations;

namespace Cinema.Data.Models
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        public int HallId { get; set; }
        [Required]
        public virtual Hall Hall { get; set; }
    }
}