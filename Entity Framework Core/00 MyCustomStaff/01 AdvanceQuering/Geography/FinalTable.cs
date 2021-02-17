using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    [Keyless]
    [Table("FinalTable")]
    public partial class FinalTable
    {
        public int MountainId { get; set; }
        [Column("Count between 5000 - 6000")]
        public int? CountBetween50006000 { get; set; }
    }
}
