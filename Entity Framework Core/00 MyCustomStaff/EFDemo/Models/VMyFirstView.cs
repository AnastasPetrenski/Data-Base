using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EFDemoDBFirst.Models
{
    [Keyless]
    public partial class VMyFirstView
    {
        [Column(TypeName = "smalldatetime")]
        public DateTime HireDate { get; set; }
        [StringLength(2)]
        public string DayOfWeek { get; set; }
        public int? NumberOfWeek { get; set; }
        [StringLength(30)]
        public string Day { get; set; }
        public int? HireDay { get; set; }
        public int? HireMonth { get; set; }
        public int? HireYear { get; set; }
        [Column(TypeName = "money")]
        public decimal Salary { get; set; }
    }
}
