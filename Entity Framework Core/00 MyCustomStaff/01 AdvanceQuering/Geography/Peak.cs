using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    public partial class Peak
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string PeakName { get; set; }
        public int Elevation { get; set; }
        public int MountainId { get; set; }

        [ForeignKey(nameof(MountainId))]
        [InverseProperty("Peaks")]
        public virtual Mountain Mountain { get; set; }
    }
}
