using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Import
{
    [XmlType("AnimalAid")]
    public class XmlProcedureAnimalAid
    {
        [XmlElement("Name")]
        public string Name { get; set; }
    }
}