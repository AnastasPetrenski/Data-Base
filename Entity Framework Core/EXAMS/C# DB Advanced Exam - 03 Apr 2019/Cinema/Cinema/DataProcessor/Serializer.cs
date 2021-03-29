namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context
                .Movies
                .Where(m => m.Rating >= rating && m.Projections.Any(p => p.Tickets.Any(c => c.Customer != null)))
                .OrderByDescending(m => m.Rating)
                .ThenByDescending(m => m.Projections.Sum(x => x.Tickets.Sum(t => t.Price)))
                .Select(m => new 
                {
                    Name = m.Title,
                    Rating = m.Rating.ToString("f2"),
                    TotalIncomes = m.Projections.Sum(x => x.Tickets.Sum(t => t.Price)).ToString("f2"),
                    ticketsCount = m.Projections.Sum(x => x.Tickets.Count), //.Where(x => x > 0)
                    customers = m.Projections
                        .SelectMany(x => x.Tickets)
                            .Select(t => new 
                            {
                                FirstName = t.Customer.FirstName,
                                LastName = t.Customer.LastName,
                                Balance = t.Customer.Balance.ToString("f2"),
                                TicketPrice = t.Price
                            })
                            .OrderByDescending(c => c.Balance)
                            .ThenBy(c => c.FirstName)
                            .ThenBy(c => c.LastName)
                            .ToList()
                            
                })
                .Take(10)
                .ToArray();

            var json = JsonConvert.SerializeObject(movies, Newtonsoft.Json.Formatting.Indented);

            return json;
             
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var dtos = context
                .Customers
                .Where(c => c.Age >= age)
                .Select(c => new XmlCustomerDto()
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = c.Tickets.Sum(t => t.Price),
                    SpentTime = new TimeSpan(c.Tickets.Sum(t => t.Projection.Movie.Duration.Ticks)).ToString(@"hh\:mm\:ss")
                })
                .OrderByDescending(c => c.SpentMoney)
                .Take(10)
                .ToList();

            var serializer = new XmlSerializer(typeof(List<XmlCustomerDto>), new XmlRootAttribute("Customers"));
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });
            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), dtos, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}