using System.Xml.Serialization;

namespace CarDealer.Models.ImportsDtos
{
    [XmlType("partId")]
    public class ImportCarDtoPartId
    {
        [XmlAttribute("id")]
        public int PartId { get; set; }
    }
}
