using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    public partial class Mountain
    {
        public Mountain()
        {
            MountainsCountries = new HashSet<MountainsCountry>();
            Peaks = new HashSet<Peak>();
            Rivers = new HashSet<River>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string MountainRange { get; set; }

        [InverseProperty(nameof(MountainsCountry.Mountain))]
        public virtual ICollection<MountainsCountry> MountainsCountries { get; set; }
        [InverseProperty(nameof(Peak.Mountain))]
        public virtual ICollection<Peak> Peaks { get; set; }
        [InverseProperty(nameof(River.Mountain))]
        public virtual ICollection<River> Rivers { get; set; }
    }
}
