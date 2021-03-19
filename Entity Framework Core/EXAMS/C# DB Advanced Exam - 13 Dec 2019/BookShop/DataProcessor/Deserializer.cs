namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ImportBookDto[]), new XmlRootAttribute("Books"));
            var booksDtos = serializer.Deserialize(new StringReader(xmlString)) as ImportBookDto[];

            var checkedBooks = new List<Book>();

            foreach (var dto in booksDtos)
            {
                if (!IsValid(dto))               
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime publishedOn;
                var isValidDate = DateTime.TryParseExact(dto.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out publishedOn);

                if (!isValidDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var book = new Book()
                {
                    Name = dto.Name,
                    Genre = (Genre)dto.Genre,
                    Price = dto.Price,
                    Pages = dto.Pages,
                    PublishedOn = publishedOn
                };

                sb.AppendLine(String.Format(SuccessfullyImportedBook, book.Name, book.Price));
                checkedBooks.Add(book);
            }

            //context.Books.AddRange(checkedBooks);
            //context.SaveChanges();

            Console.WriteLine(sb.ToString().TrimEnd());
            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var authorDtos = JsonConvert.DeserializeObject<ImportAuthorDto[]>(jsonString);

            var sb = new StringBuilder();

            var authors = new List<Author>();

            foreach (var dto in authorDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if(authors.Any(a => a.Email == dto.Email))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var author = new Author()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Phone = dto.Phone,
                    Email = dto.Email,
                };

                var books = context.Books.Select(b => b.Id).ToList();
                
                foreach (var dtoBookId in dto.Books)
                {
                    var book = context.Books.FirstOrDefault(b => b.Id == dtoBookId.Id);
                    int id = dtoBookId.Id ?? default(int);

                    if (id > 0 && books.Contains(id))
                    {
                        Console.WriteLine("Valid ID");
                    }

                    if (book != null)
                    {
                        author.AuthorsBooks.Add(new AuthorBook()
                        {
                            Author = author,
                            Book = book
                        });
                    }
                }

                if (author.AuthorsBooks.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                }
                else
                {
                    authors.Add(author);
                    sb.AppendLine(string.Format(SuccessfullyImportedAuthor, $"{author.FirstName} {author.LastName}", author.AuthorsBooks.Count()));
                }
            }

            //context.Authors.AddRange(authors);
            //context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}