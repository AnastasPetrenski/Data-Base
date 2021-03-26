using System.Xml.Serialization;

namespace Stations.DataProcessor.Dto.Import
{
    [XmlType("Card")]
    public class XmlCardTickedDto
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }
}