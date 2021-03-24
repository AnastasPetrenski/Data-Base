using Newtonsoft.Json;

namespace Stations.DataProcessor.Dto.Import
{
    public class TripDto
    {
        [JsonProperty("Train")]
        public string TrainNumber { get; set; }
        public string OriginStation { get; set; }
        public string DestinationStation { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string Status { get; set; }
        public string TimeDifference { get; set; }
    }
}
