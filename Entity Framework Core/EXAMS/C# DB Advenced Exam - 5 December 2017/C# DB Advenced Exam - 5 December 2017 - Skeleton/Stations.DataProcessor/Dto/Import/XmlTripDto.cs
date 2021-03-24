using System.Xml.Serialization;

namespace Stations.DataProcessor.Dto.Import
{
    [XmlType("Trip")]
    public class XmlTripDto
    {
        [XmlElement("OriginStation")]
        public string OriginStation { get; set; }

        public string DestinationStation { get; set; }

        public string DepartureTime { get; set; }
    }
}