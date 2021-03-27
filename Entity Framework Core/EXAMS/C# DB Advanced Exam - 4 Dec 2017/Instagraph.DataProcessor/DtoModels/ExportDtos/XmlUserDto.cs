using System.Xml.Serialization;

namespace Instagraph.DataProcessor.DtoModels.ExportDtos
{
    //[XmlRoot("users")]
    [XmlType("user")]
    public class XmlUserDto
    {
        [XmlElement("Username")]
        public string Username { get; set; }

        [XmlElement("MostComments")]
        public int MostComments { get; set; }
    }
}
