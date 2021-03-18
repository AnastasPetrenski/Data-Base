using System.Xml.Serialization;

namespace CarDealer.Models.ExportsDtos
{
    [XmlType("part")]
    public class ExportCarPartsArrayDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }
}
