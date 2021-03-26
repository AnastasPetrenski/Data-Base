using System.ComponentModel.DataAnnotations;

namespace Instagraph.DataProcessor.DtoModels.ImportDtos
{
    public class JsonPictureDto
    {
        public string Path { get; set; }
  
        public decimal Size { get; set; }
    }
}
