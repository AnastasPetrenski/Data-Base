using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    [Keyless]
    public partial class VPeaksBetween7000And8000Meter
    {
        public int MountainId { get; set; }
        [Column("Count between 7000 - 8000")]
        public int? CountBetween70008000 { get; set; }
    }
}
