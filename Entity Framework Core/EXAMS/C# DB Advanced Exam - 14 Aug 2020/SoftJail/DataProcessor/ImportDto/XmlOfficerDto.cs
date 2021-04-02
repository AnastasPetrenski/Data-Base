using SoftJail.Data.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class XmlOfficerDto
    {
        [XmlElement("Name")]
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string FullName { get; set; }

        [XmlElement("Money")]
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Salary { get; set; }

        [XmlElement("Position")]
        [Required]
        [EnumDataType(typeof(Position))]
        public string Position { get; set; }

        [XmlElement("Weapon")]
        [Required]
        [EnumDataType(typeof(Weapon))]
        public string Weapon { get; set; }

        [XmlElement("DepartmentId")]
        [Required]
        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public XmlPrisonerDto[] Prisoners { get; set; }
    }
}
