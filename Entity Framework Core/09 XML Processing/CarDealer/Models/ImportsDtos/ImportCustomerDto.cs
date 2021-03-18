using System.Xml.Serialization;

namespace CarDealer.Models.ImportsDtos
{
    [XmlType("Customer")]
    public class ImportCustomerDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("birthDate")]
        public string BirthDate { get; set; }

        [XmlElement("isYoungDriver")]
        public bool IsYoungDriver { get; set; }
    }
}

//< name > Emmitt Benally </ name >
//< birthDate > 1993 - 11 - 20T00: 00:00 </ birthDate >
//< isYoungDriver > true </ isYoungDriver >