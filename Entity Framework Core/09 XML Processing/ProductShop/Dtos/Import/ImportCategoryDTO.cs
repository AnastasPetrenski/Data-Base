using System.Xml.Serialization;

namespace ProductShop.Dtos.Import
{
    [XmlRoot("Categories")]
    [XmlType("Category")]
    public class ImportCategoryDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
