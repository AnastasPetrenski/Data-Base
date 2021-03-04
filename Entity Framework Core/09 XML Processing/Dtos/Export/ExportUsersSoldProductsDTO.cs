using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlRoot("Users")]
    [XmlType("User")]
    public class ExportUsersSoldProductsDTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlArray("soldProducts")]
        public UserProductDTO[] SoldProducts { get; set; }
    }

    [XmlType("Product")]
    public class UserProductDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
