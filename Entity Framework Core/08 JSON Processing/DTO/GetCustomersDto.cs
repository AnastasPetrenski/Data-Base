using Newtonsoft.Json;

namespace CarDealer.DTO
{
    public class GetCustomersDto
    {
        public string Name { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/MM/yyyy}")]
        [JsonProperty("BirthDate")]
        public string BirthDate { get; set; }

        public bool IsYoungDriver { get; set; }
    }
}
