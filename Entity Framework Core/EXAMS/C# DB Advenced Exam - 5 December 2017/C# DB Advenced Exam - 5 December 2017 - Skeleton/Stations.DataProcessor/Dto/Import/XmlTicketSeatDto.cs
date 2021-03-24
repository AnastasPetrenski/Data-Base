using System.Xml.Serialization;

namespace Stations.DataProcessor.Dto.Import
{
    [XmlType("Ticket")]
    public class XmlTicketSeatDto
    {
        [XmlAttribute("seat")]
        public string Seat { get; set; }

        [XmlAttribute("price")]
        public string Price { get; set; }

        [XmlElement("Trip")]
        public XmlTripDto Trip { get; set; }

        [XmlElement("Card")]
        public XmlCardTickedDto Card { get; set; }
    }
}
