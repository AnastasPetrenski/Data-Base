using System.Xml.Serialization;

namespace ProductShop.Dtos.Export.ExportUserProducts
{

    public class ExportProductCountDTO
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public ExportProductsDTO[] Products { get; set; }

    }
}
