using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Instagraph.DataProcessor.DtoModels.ImportDtos
{
    [XmlType("post")]
    public class XmlPostDto
    {
        [Required]
        [XmlElement("caption")]
        public string Caption { get; set; }

        [Required]
        [XmlElement("user")]
        public string Username { get; set; }

        [Required]
        [XmlElement("picture")]
        public string Picture { get; set; }
    }
}
