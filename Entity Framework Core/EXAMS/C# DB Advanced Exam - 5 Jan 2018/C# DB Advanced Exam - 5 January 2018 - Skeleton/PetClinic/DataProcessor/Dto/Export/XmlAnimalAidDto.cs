using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Export
{
    [XmlType("AnimalAid")]
    public class XmlAnimalAidDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
