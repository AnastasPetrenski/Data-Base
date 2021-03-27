using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Export
{
    [XmlType("Category")]
    public class XmlExportCategoryDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("MostPopularItem")]
        public XmlExportMostPopularDto MostPopularItem { get; set; }
    }
}
