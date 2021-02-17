using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF_03_Intro.Geography
{
    [Keyless]
    public partial class VAbove8000Meter
    {
        public int MountainId { get; set; }
        [Column("Count_Above_8000_meters")]
        public int? CountAbove8000Meters { get; set; }
    }
}
