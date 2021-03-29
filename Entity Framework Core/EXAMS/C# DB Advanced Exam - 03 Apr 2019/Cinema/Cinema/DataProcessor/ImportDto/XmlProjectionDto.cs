using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Projection")]
    public class XmlProjectionDto
    {
        [XmlElement("MovieId")]
        public int MovieId { get; set; }

        [XmlElement("HallId")]
        public int HallId { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }
    }
}
