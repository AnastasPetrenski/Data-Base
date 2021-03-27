using System.Collections.Generic;

namespace Stations.DataProcessor.Dto.Import
{
    public class TrainDto
    {
        public string TrainNumber { get; set; }

        public string Type { get; set; }

        public ICollection<TrainSeatDto> Seats { get; set; }
    }
}
