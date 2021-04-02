using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.DataProcessor.ImportDto
{
    public class JsonDepartmentDto
    {
        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        public ICollection<JsonCellDto> Cells { get; set; }
    }
}
