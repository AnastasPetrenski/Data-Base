using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Import
{
    [XmlType("Item")]
    public class XmlItemDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        [XmlElement("Name")]
        public string ItemName { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [XmlElement("Quantity")]
        public int Quantity { get; set; }
    }
}