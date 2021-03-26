using System.ComponentModel.DataAnnotations;

namespace Instagraph.DataProcessor.DtoModels.ImportDtos
{
    public class JsonUserDto
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string ProfilePicture { get; set; }
    }
}
