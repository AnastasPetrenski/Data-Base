using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    [Keyless]
    public partial class VPeaksBetween6000And7000Meter
    {
        public int MountainId { get; set; }
        [Column("Count between 6000 - 7000")]
        public int? CountBetween60007000 { get; set; }
    }
}
