using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    public partial class CountriesRiver
    {
        [Key]
        public int RiverId { get; set; }
        [Key]
        [StringLength(2)]
        public string CountryCode { get; set; }

        [ForeignKey(nameof(CountryCode))]
        [InverseProperty(nameof(Country.CountriesRivers))]
        public virtual Country CountryCodeNavigation { get; set; }
        [ForeignKey(nameof(RiverId))]
        [InverseProperty("CountriesRivers")]
        public virtual River River { get; set; }
    }
}
