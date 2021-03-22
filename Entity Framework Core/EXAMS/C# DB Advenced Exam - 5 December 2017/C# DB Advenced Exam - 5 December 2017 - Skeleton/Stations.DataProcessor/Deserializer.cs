using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stations.Data;
using Stations.DataProcessor.Dto.Import;
using Stations.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Stations.DataProcessor
{
    public static class Deserializer
    {
        private const string FailureMessage = "Invalid data format.";

        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportStations(StationsDbContext context, string jsonString)
        {
            var stationDtos = JsonConvert.DeserializeObject<StationDto[]>(jsonString);

            List<Station> stations = new List<Station>();
            var sb = new StringBuilder();

            foreach (var dto in stationDtos)
            {
                Station station = new Station()
                {
                    Name = dto.Name,
                    Town = dto.Town
                };

                if (stations.Any(s => s.Name == station.Name) || station.Name is null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                if (station.Town is null)
                {
                    station.Town = station.Name;
                }

                if (IsValid(station))
                {
                    stations.Add(station);

                    sb.AppendLine(String.Format(SuccessMessage, station.Name));
                }
                else
                {
                    sb.AppendLine(FailureMessage);
                }

            }

            context.Stations.AddRange(stations);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportClasses(StationsDbContext context, string jsonString)
        {
            var seatClassDtos = JsonConvert.DeserializeObject<SeatingClassDto[]>(jsonString);

            var x = seatClassDtos
                .Select(s => new SeatingClass()
                {
                    Name = s.Name,
                    Abbreviation = s.Abbreviation
                })
                .ToList()
                .Distinct();

            var seats = new List<SeatingClass>();
            var sb = new StringBuilder();

            foreach (var dto in seatClassDtos)
            {
                var seat = new SeatingClass()
                {
                    Name = dto.Name,
                    Abbreviation = dto.Abbreviation
                };

                if (!IsValid(seat))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                if (seats.Any(s => s.Name == seat.Name) || 
                    seats.Any(s => s.Abbreviation == seat.Abbreviation))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                seats.Add(seat);
                sb.AppendLine(String.Format(SuccessMessage, seat.Name));
            }

            context.SeatingClasses.AddRange(seats);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTrains(StationsDbContext context, string jsonString)
        {
            return null;
        }

        public static string ImportTrips(StationsDbContext context, string jsonString)
        {
            return null;
        }

        public static string ImportCards(StationsDbContext context, string xmlString)
        {
            return null;
        }

        public static string ImportTickets(StationsDbContext context, string xmlString)
        {
            return null;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}