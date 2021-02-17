using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Models
{
    [Keyless]
    [Table("Grouped")]
    public partial class Grouped
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        public string Department { get; set; }
        [Column("ProjectID")]
        public int? ProjectId { get; set; }
        [StringLength(50)]
        public string Projects { get; set; }
        [Column(TypeName = "money")]
        public decimal Salary { get; set; }
    }
}
