using System.Xml.Serialization;

namespace Stations.DataProcessor.Dto.Export
{
    [XmlRoot("Tickets")]
    [XmlType("Ticket")]
    public class XmlTicketDto
    {
        [XmlElement("OriginStation")]
        public string OriginStation { get; set; }

        [XmlElement("DestinationStation")]
        public string DestinationStation { get; set; }

        [XmlElement("DepartureTime")]
        public string DepartureTime { get; set; }
    }
}