using System.ComponentModel.DataAnnotations;

namespace Cinema.Data.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        public int CustomerId { get; set; }
        [Required]
        public virtual Customer Customer { get; set; }

        public int ProjectionId { get; set; }
        [Required]
        public virtual Projection Projection { get; set; }
    }
}