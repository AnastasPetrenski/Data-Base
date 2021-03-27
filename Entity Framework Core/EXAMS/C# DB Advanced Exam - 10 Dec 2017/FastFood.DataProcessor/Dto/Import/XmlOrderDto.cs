using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Import
{
    [XmlType("Order")]
    public class XmlOrderDto
    {
        [Required]
        [XmlElement("Customer")]
        public string Customer { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        [XmlElement("Employee")]
        public string EmployeeName { get; set; }

        [Required]
        [XmlElement("DateTime")]
        public string DateTime { get; set; }
                
        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlArray("Items")]
        public XmlItemDto[] Items { get; set; }
    }
}
