namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2:f2}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<JsonMovieDto[]>(jsonString);
            var movies = new List<Movie>();

            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (movies.Any(m => m.Title == dto.Title))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                TimeSpan.TryParseExact(dto.Duration, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out TimeSpan duration);

                Enum.TryParse<Genre>(dto.Genre, out Genre genre);

                movies.Add(new Movie()
                {
                    Title = dto.Title,
                    Genre = genre,
                    Duration = duration,
                    Rating = dto.Rating,
                    Director = dto.Director
                });

                sb.AppendLine(string.Format(SuccessfulImportMovie, dto.Title, genre, dto.Rating));
            }

            context.Movies.AddRange(movies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<JsonHallDto[]>(jsonString);

            var sb = new StringBuilder();
            var halls = new List<Hall>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hall = new Hall()
                {
                    Name = dto.Name,
                    Is4Dx = dto.Is4Dx,
                    Is3D = dto.Is3D
                };

                for (int i = 0; i < dto.Seats; i++)
                {
                    hall.Seats.Add(new Seat());
                    //{
                    //    Hall = hall
                    //});
                }

                var projectionType = string.Empty;

                if (hall.Is3D && hall.Is4Dx)
                {
                    projectionType = "4Dx/3D";
                }
                else if (hall.Is4Dx)
                {
                    projectionType = "4Dx";
                }
                else if (hall.Is3D)
                {
                    projectionType = "3D";
                }
                else
                {
                    projectionType = "Normal";
                }

                halls.Add(hall);
                sb.AppendLine(string.Format(SuccessfulImportHallSeat, hall.Name, projectionType, hall.Seats.Count));
            }

            context.Halls.AddRange(halls);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(XmlProjectionDto[]), new XmlRootAttribute("Projections"));
            var dtos = serializer.Deserialize(new StringReader(xmlString)) as XmlProjectionDto[];

            var sb = new StringBuilder();
            var projections = new List<Projection>();

            foreach (var dto in dtos)
            {
                var movie = context.Movies.FirstOrDefault(x => x.Id == dto.MovieId);

                if (movie is null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hall = context.Halls.FirstOrDefault(x => x.Id == dto.HallId);

                if (hall is null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime.TryParseExact(dto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

                projections.Add(new Projection()
                {
                    Movie = movie,
                    Hall = hall,
                    DateTime = date
                });

                sb.AppendLine(string.Format(SuccessfulImportProjection, movie.Title, date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));
            }

            context.Projections.AddRange(projections);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(XmlCustomerDto[]), new XmlRootAttribute("Customers"));
            var dtos = serializer.Deserialize(new StringReader(xmlString)) as XmlCustomerDto[];

            var sb = new StringBuilder();
            var customers = new List<Customer>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var customer = new Customer()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age,
                    Balance = dto.Balance                    
                };

                foreach (var ticketDto in dto.Tickets)
                {
                    if (!IsValid(ticketDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var projection = context.Projections.FirstOrDefault(p => p.Id == ticketDto.ProjectionId);

                    if (projection is null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var ticket = new Ticket()
                    {
                        Customer = customer,
                        Projection = projection,
                        Price = ticketDto.Price
                    };

                    customer.Tickets.Add(ticket);
                }

                customers.Add(customer);
                sb.AppendLine(
                    string.Format(
                        SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count));
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return isValid;
        }
    }
}