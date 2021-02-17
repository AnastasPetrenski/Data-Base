using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    public partial class Country
    {
        public Country()
        {
            CountriesRivers = new HashSet<CountriesRiver>();
            MountainsCountries = new HashSet<MountainsCountry>();
        }

        [Key]
        [StringLength(2)]
        public string CountryCode { get; set; }
        [Required]
        [StringLength(3)]
        public string IsoCode { get; set; }
        [Required]
        [StringLength(45)]
        public string CountryName { get; set; }
        [StringLength(3)]
        public string CurrencyCode { get; set; }
        [Required]
        [StringLength(2)]
        public string ContinentCode { get; set; }
        public int Population { get; set; }
        public int AreaInSqKm { get; set; }
        [Required]
        [StringLength(30)]
        public string Capital { get; set; }

        [ForeignKey(nameof(ContinentCode))]
        [InverseProperty(nameof(Continent.Countries))]
        public virtual Continent ContinentCodeNavigation { get; set; }
        [ForeignKey(nameof(CurrencyCode))]
        [InverseProperty(nameof(Currency.Countries))]
        public virtual Currency CurrencyCodeNavigation { get; set; }
        [InverseProperty(nameof(CountriesRiver.CountryCodeNavigation))]
        public virtual ICollection<CountriesRiver> CountriesRivers { get; set; }
        [InverseProperty(nameof(MountainsCountry.CountryCodeNavigation))]
        public virtual ICollection<MountainsCountry> MountainsCountries { get; set; }
    }
}
