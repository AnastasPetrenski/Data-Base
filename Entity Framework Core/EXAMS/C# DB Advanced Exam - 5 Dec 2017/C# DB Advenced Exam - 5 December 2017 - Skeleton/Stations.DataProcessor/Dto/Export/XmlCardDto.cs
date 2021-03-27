using System.Xml.Serialization;

namespace Stations.DataProcessor.Dto.Export
{
    [XmlType("Card")]
    public class XmlCardDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlArray("Tickets")]
        public XmlTicketDto[] Tickets { get; set; }
    }
}
