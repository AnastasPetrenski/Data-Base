using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Models
{
    [Keyless]
    [Table("Filter")]
    public partial class Filter
    {
        [Column("DepartmentID")]
        public int DepartmentId { get; set; }
        [Column(TypeName = "money")]
        public decimal? Average { get; set; }
    }
}
