using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    public partial class River
    {
        public River()
        {
            CountriesRivers = new HashSet<CountriesRiver>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string RiverName { get; set; }
        public int Length { get; set; }
        public int DrainageArea { get; set; }
        public int AverageDischarge { get; set; }
        [Required]
        [StringLength(50)]
        public string Outflow { get; set; }
        [Column("MountainID")]
        public int? MountainId { get; set; }

        [ForeignKey(nameof(MountainId))]
        [InverseProperty("Rivers")]
        public virtual Mountain Mountain { get; set; }
        [InverseProperty(nameof(CountriesRiver.River))]
        public virtual ICollection<CountriesRiver> CountriesRivers { get; set; }
    }
}
