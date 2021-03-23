using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stations.Data;
using Stations.DataProcessor.Dto.Import;
using Stations.Models;
using Stations.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

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
            var dtos = JsonConvert.DeserializeObject<TrainDto[]>(jsonString);

            var trains = new List<Train>();
            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {

                var train = new Train()
                {
                    TrainNumber = dto.TrainNumber,
                    Type = dto.Type is null ? TrainType.HighSpeed : Enum.Parse<TrainType>(dto.Type),
                };

                if (!IsValid(train))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                if (trains.Any(t => t.TrainNumber == train.TrainNumber))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                if (dto.Seats is null)
                {
                    dto.Seats = new List<TrainSeatDto>();
                }

                bool isValid = true;
                foreach (var seatDto in dto.Seats)
                {
                    var seat = context.SeatingClasses.FirstOrDefault(x => x.Name == seatDto.Name &&
                                                                    x.Abbreviation == seatDto.Abbreviation);

                    if (seat is null || seatDto.Quantity < 0 || seatDto.Quantity is null)
                    {
                        sb.AppendLine(FailureMessage);
                        isValid = false;
                        break;
                    }

                    train.TrainSeats.Add(new TrainSeat()
                    {
                        Train = train,
                        SeatingClass = seat,
                        Quantity = seatDto.Quantity.Value
                    });
                }

                if (isValid)
                {
                    trains.Add(train);
                    sb.AppendLine(String.Format(SuccessMessage, train.TrainNumber));
                }

            }

            context.Trains.AddRange(trains);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTrips(StationsDbContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<TripDto[]>(jsonString);

            var trips = new List<Trip>();

            var sb = new StringBuilder();

            var trains = context.Trains.ToList();
            var stations = context.Stations.ToList();

            foreach (var dto in dtos)
            {
                //var train = context.Trains.FirstOrDefault(t => t.TrainNumber == dto.TrainNumber);
                var train = trains.FirstOrDefault(t => t.TrainNumber == dto.TrainNumber);

                if (train is null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var originStation = stations.FirstOrDefault(x => x.Name == dto.OriginStation);

                if (originStation is null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var destinationStation = stations.FirstOrDefault(x => x.Name == dto.DestinationStation);

                if (destinationStation is null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                DateTime departureTime;
                var isDepartureValid = DateTime.TryParseExact(dto.DepartureTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out departureTime);

                DateTime arrivalTime;
                var isArrivalValid = DateTime.TryParseExact(dto.ArrivalTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalTime);

                if (!isDepartureValid || !isArrivalValid || departureTime > arrivalTime)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                TimeSpan timeSpan;

                if (dto.TimeDifference != null)
                {
                    timeSpan = TimeSpan.ParseExact(dto.TimeDifference, @"hh\:mm", CultureInfo.InvariantCulture);
                }

                TripStatus status;

                if (string.IsNullOrEmpty(dto.Status))
                {
                    status = TripStatus.OnTime;
                }
                else
                {
                    Enum.TryParse(dto.Status, out status);
                }

                var trip = new Trip()
                {
                    OriginStation = originStation,
                    DestinationStation = destinationStation,
                    DepartureTime = departureTime,
                    ArrivalTime = arrivalTime,
                    Train = train,
                    Status = status,
                    TimeDifference = timeSpan
                };

                if (IsValid(trip))
                {
                    trips.Add(trip);
                    sb.AppendLine($"Trip from {trip.OriginStation.Name} to {trip.DestinationStation.Name} imported.");
                }

            }

            context.Trips.AddRange(trips);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCards(StationsDbContext context, string xmlString)
        {
            //Exception for [XmlAnyElement("Name")]
            //InvalidOperationException: Cannot serialize member of type System.String: XmlAnyElement 
            //can only be used with classes of type XmlNode or a type deriving from XmlNode.

            var serializer = new XmlSerializer(typeof(XmlCardDto[]), new XmlRootAttribute("Cards"));
            var dtos = serializer.Deserialize(new StringReader(xmlString)) as XmlCardDto[];

            var sb = new StringBuilder();
            var customerCards = new List<CustomerCard>();

            var cards = dtos
                .Select(x => new CustomerCard()
                {
                    Name = x.Name,
                    Age = x.Age,
                    Type = x.CardType is null ? CardType.Normal : (CardType)Enum.Parse(typeof(CardType), x.CardType)
                })
                .Where(x => IsValid(x))
                .ToList();

            foreach (var dto in dtos)
            {
                var customerCard = new CustomerCard()
                {
                    Name = dto.Name,
                    Age = dto.Age,
                    Type = dto.CardType is null ? CardType.Normal : (CardType)Enum.Parse(typeof(CardType), dto.CardType)
                };

                if (IsValid(customerCard))
                {
                    customerCards.Add(customerCard);
                    sb.AppendLine(String.Format(SuccessMessage, customerCard.Name));
                }
                else
                {
                    sb.AppendLine(FailureMessage);
                }
            }

            context.CustomerCards.AddRange(customerCards);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTickets(StationsDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(XmlTicketSeatDto[]), new XmlRootAttribute("Tickets"));
            var dtos = serializer.Deserialize(new StringReader(xmlString)) as XmlTicketSeatDto[];

            //N+1 problem escaping
            var trips = context
                .Trips
                .Include(x => x.OriginStation)
                .Include(x => x.DestinationStation)
                .Include(x => x.Train)
                .ToList();

            var seatAbbreviations = context.SeatingClasses.Select(x => x.Abbreviation).ToList();
            var tickets = new List<Ticket>();

            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                DateTime departureTime;
                var result = DateTime.TryParseExact(dto.Trip.DepartureTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out departureTime);

                //if we use context.Trips N+1 problem occur
                var trip = trips.FirstOrDefault(x => x.OriginStation.Name == dto.Trip.OriginStation
                                                  && x.DestinationStation.Name == dto.Trip.DestinationStation
                                                  && x.DepartureTime == departureTime);

                if (trip is null)
                {
                    //Ignore the entity or append error message and ignore???
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                string seatClassAbbreviation = dto.Seat.Substring(0, 2);
                string seatNumberString = dto.Seat.Substring(2);

                if (!seatAbbreviations.Contains(seatClassAbbreviation))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                int seatNumber;

                if (!int.TryParse(seatNumberString, out seatNumber))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var trainSeats = context.TrainSeats.Include(x => x.SeatingClass).Where(x => x.TrainId == trip.TrainId).ToArray();
                var seat = trainSeats.FirstOrDefault(x =>
                    x.SeatingClass.Abbreviation == seatClassAbbreviation
                    && x.Quantity >= seatNumber
                    && seatNumber > 0);

                if (seat == null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                CustomerCard customerCard = null;

                if (dto.Card != null)
                {
                    customerCard = context.CustomerCards.FirstOrDefault(x => x.Name == dto.Card.Name);

                    if (customerCard is null)
                    {
                        sb.AppendLine(FailureMessage);
                        continue;
                    }
                }

                var ticket = new Ticket()
                {
                    Price = decimal.Parse(dto.Price),
                    SeatingPlace = dto.Seat,
                    Trip = trip,
                    CustomerCard = customerCard
                };

                tickets.Add(ticket);

                sb.AppendLine($"Ticket from {ticket.Trip.OriginStation.Name} to {ticket.Trip.DestinationStation.Name} departing at {departureTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} imported.");
            }

            context.Tickets.AddRange(tickets);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
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