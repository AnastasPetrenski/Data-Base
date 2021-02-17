using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    public partial class MountainsCountry
    {
        [Key]
        public int MountainId { get; set; }
        [Key]
        [StringLength(2)]
        public string CountryCode { get; set; }

        [ForeignKey(nameof(CountryCode))]
        [InverseProperty(nameof(Country.MountainsCountries))]
        public virtual Country CountryCodeNavigation { get; set; }
        [ForeignKey(nameof(MountainId))]
        [InverseProperty("MountainsCountries")]
        public virtual Mountain Mountain { get; set; }
    }
}
