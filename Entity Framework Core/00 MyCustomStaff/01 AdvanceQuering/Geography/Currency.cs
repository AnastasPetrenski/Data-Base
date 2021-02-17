using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    public partial class Currency
    {
        public Currency()
        {
            Countries = new HashSet<Country>();
        }

        [Key]
        [StringLength(3)]
        public string CurrencyCode { get; set; }
        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [InverseProperty(nameof(Country.CurrencyCodeNavigation))]
        public virtual ICollection<Country> Countries { get; set; }
    }
}
