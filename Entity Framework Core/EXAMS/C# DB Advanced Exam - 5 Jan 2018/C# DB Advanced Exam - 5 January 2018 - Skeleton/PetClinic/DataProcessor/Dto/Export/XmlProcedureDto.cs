using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Export
{
    [XmlType("Procedure")]
    public class XmlProcedureDto
    {
        [XmlElement("Passport")]
        public string Passport { get; set; }

        [XmlElement("OwnerNumber")]
        public string OwnerNumber { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        public XmlAnimalAidDto[] AnimalAids { get; set; }

        [XmlElement("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }
}
