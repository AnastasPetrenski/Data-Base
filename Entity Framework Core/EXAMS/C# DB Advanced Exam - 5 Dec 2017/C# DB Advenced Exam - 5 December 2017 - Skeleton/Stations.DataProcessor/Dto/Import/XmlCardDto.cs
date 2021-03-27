using System.Xml.Serialization;

namespace Stations.DataProcessor.Dto.Import
{
    [XmlType("Card")]
    public class XmlCardDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Age")]
        public int Age { get; set; }

        [XmlElement("CardType")]
        public string CardType { get; set; }
    }
}
