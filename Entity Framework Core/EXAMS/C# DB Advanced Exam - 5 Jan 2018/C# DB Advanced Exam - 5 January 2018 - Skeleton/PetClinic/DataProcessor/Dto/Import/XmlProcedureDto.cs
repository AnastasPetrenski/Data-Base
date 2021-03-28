using System.Collections.Generic;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Import
{
    [XmlType("Procedure")]
    public class XmlProcedureDto
    {
        [XmlElement("Vet")]
        public string Vet { get; set; }

        [XmlElement("Animal")]
        public string Animal { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        public XmlProcedureAnimalAid[] AnimalAids { get; set; }
    }
}
