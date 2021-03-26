using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Instagraph.DataProcessor.DtoModels.ImportDtos
{
    [XmlType("comment")]
    public class XmlCommentDto
    {
        [Required]
        [MaxLength(250)]
        [XmlElement("content")]
        public string Content { get; set; }

        [Required]
        [XmlElement("user")]
        public string Username { get; set; }

        [Required]
        [XmlElement("post")]
        public XmlCommentPostDto Post { get; set; }
    }
}
