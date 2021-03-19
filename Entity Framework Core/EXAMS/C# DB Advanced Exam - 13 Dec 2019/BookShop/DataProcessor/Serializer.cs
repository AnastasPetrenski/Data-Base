namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authorDtos = context.Authors
                                 .Select(a => new
                                 {
                                     AuthorName = $"{a.FirstName} {a.LastName}",
                                     Books = a.AuthorsBooks
                                              .Select(c => c.Book)
                                              .OrderByDescending(b => b.Price)
                                              .Select(b => new
                                     {
                                         BookName = b.Name,
                                         BookPrice = b.Price.ToString("F2")
                                     })
                                     .ToList()
                                 })
                                 .ToList() //<== For Judge 25/25
                                 .OrderByDescending(a => a.Books.Count())
                                 .ThenBy(a => a.AuthorName)
                                 .ToList();

            var authors = JsonConvert.SerializeObject(authorDtos, Formatting.Indented);

            return authors;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books = context
                .Books
                .Where(b => b.PublishedOn < date && b.Genre == Genre.Science)
                .OrderByDescending(b => b.Pages)
                .ThenByDescending(b => b.PublishedOn)
                .Take(10)
                .Select(b => new ExportBookDto()
                {
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                    Pages = b.Pages.ToString()
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportBookDto[]), new XmlRootAttribute("Books"));
            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });

            serializer.Serialize(new StringWriter(sb), books, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}