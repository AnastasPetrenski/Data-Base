using Newtonsoft.Json;
using Stations.Data;
using Stations.DataProcessor.Dto.Export;
using Stations.Models.Enums;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Stations.DataProcessor
{
    public class Serializer
    {
        public static string ExportDelayedTrains(StationsDbContext context, string dateAsString)
        {
            var timeConstraint = DateTime.ParseExact(dateAsString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var trains = context
                .Trains
                .Where(t => t.Trips.Any(x => x.Status.ToString() == "Delayed" && 
                                             x.DepartureTime.Ticks <= timeConstraint.Ticks))
                .Select(t => new JsonTrainDto
                {
                    TrainNumber = t.TrainNumber,
                    DelayedTimes = t.Trips.Count(x => x.Status == TripStatus.Delayed && 
                                                      x.DepartureTime <= timeConstraint),
                    MaxDelayedTime = t.Trips.Max(x => x.TimeDifference),
                })
                .OrderByDescending(t => t.DelayedTimes)
                .ThenByDescending(t => t.MaxDelayedTime)
                .ThenBy(t => t.TrainNumber)
                .ToList();

            var json = JsonConvert.SerializeObject(trains, Newtonsoft.Json.Formatting.Indented);
                       
            return json;
        }

        public static string ExportCardsTicket(StationsDbContext context, string cardType)
        {
            var cardDtos = context
                .CustomerCards
                .Where(c => c.Type.ToString() == cardType && c.BoughtTickets.Count > 0)
                .Select(c => new XmlCardDto()
                {
                    Name = c.Name,
                    Type = c.Type.ToString(),
                    Tickets = c.BoughtTickets.Select(x => new XmlTicketDto()
                    {
                        OriginStation = x.Trip.OriginStation.Name,
                        DestinationStation = x.Trip.OriginStation.Name,
                        DepartureTime = x.Trip.DepartureTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                    })
                    .ToArray()
                })
                .OrderBy(c => c.Name)
                .ToArray();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(XmlCardDto[]), new XmlRootAttribute("Cards"));
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });

            serializer.Serialize(new StringWriter(sb), cardDtos, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}