using System.Xml.Serialization;

namespace Instagraph.DataProcessor.DtoModels.ImportDtos
{

    public class XmlCommentPostDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}