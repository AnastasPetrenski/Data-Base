using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    public partial class Continent
    {
        public Continent()
        {
            Countries = new HashSet<Country>();
        }

        [Key]
        [StringLength(2)]
        public string ContinentCode { get; set; }
        [Required]
        [StringLength(50)]
        public string ContinentName { get; set; }

        [InverseProperty(nameof(Country.ContinentCodeNavigation))]
        public virtual ICollection<Country> Countries { get; set; }
    }
}
