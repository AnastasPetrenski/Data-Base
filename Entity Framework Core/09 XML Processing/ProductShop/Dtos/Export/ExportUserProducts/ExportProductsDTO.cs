using System.Xml.Serialization;

namespace ProductShop.Dtos.Export.ExportUserProducts
{
    [XmlRoot("products")]
    [XmlType("Product")]
    public class ExportProductsDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
