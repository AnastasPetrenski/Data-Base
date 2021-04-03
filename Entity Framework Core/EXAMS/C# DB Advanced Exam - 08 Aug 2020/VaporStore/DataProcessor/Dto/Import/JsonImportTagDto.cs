using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class JsonImportTagDto
    {
        [Required]
        public string TagName { get; set; }
    }
}