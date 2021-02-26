using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EFDemoDBFirst.Models
{
    [Keyless]
    public partial class VPublicDatum
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(4000)]
        public string StartDate { get; set; }
    }
}
