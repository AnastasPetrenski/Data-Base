namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //var bookId = 4;
            //var songs = db.Books.FromSqlInterpolated($"SELECT * FROM Books Where BookId <= {bookId}").ToList();

            ////Take all information
            //var minPrice = 15;
            //var books = db.Books.FromSqlRaw($"SELECT * FROM Books WHERE Price > {minPrice}").ToList();

            ////Eager loading == .Include()
            //var selectedBooks = db.Books.Include(x => x.BookCategories).Where(x => x.Price > 15).ToList();

            ////Stop tracking
            //var cur = db.Books.AsNoTracking().Where(x => x.Author.AuthorId == 1).ToList();

            ////Anonymous object or Classes which are unknown for DbContext, ChangeTracker stop tracking changes  
            //var num = db.Books.Select(x => new
            //{
            //    Name = x.Author.Books.Where(b => b.Title.Length > 3)
            //}).ToList();

            ////Z.EntityFramework.Plus Library
            ////var deleted = db.Authors.Where(x => x.Books.Count <= 1).Delete();

            //foreach (var book in books)
            //{
            //    //Explicit loading, before that Author is null
            //    db.Entry(book).Reference(x => x.Author).Load();
            //    //Stop tracking entity changes
            //    db.Entry(book).State = EntityState.Detached;

            //    System.Console.WriteLine($"{book.Author.FirstName + " " + book.Author.LastName} {book.Title} {book.Price}");
            //    System.Console.WriteLine($"{string.Join(" ", book.BookCategories)}");
            //}

            ///***************************** N+1 Problem *******************************/

            ////N+1 Problem test with Lazy Loading
            //var allBooksBelowFifty = db.Books
            //                           .Where(x => x.BookId <= 50)
            //                           .ToList();

            //foreach (var item in allBooksBelowFifty)
            //{
            //    System.Console.WriteLine($"{item.Title} + {item.Author.Books.Count}");
            //}

            ////Preventing N+1 Problem is using Select projection 
            //var allBooksBelowFiftyProjection = db.Books
            //                           .Where(x => x.BookId <= 50)
            //                           .Select(x => new { x.Title, x.Author.Books.Count})
            //                           .ToList();

            //foreach (var item in allBooksBelowFiftyProjection)
            //{
            //    System.Console.WriteLine(item.Title + " " + item.Count);
            //}

            ///***************************** Concurrency Check *******************************/
            ////Add property EarnedMoney with attribute [ConcurrencyCheck] or with FluentApi - IsConcurrencyToken(true)

            //var db2 = new BookShopContext();
            //var author = db.Authors.FirstOrDefault(x => x.AuthorId == 1);
            //author.EarnedMoney += 1000;
            //var author2 = db2.Authors.First(x => x.AuthorId == 1);


            //db.SaveChanges();

            //while (true)
            //{
            //    try
            //    {
            //        author2.EarnedMoney += 1000;
            //        db2.SaveChanges();
            //        break;
            //    }
            //    catch (System.Exception)
            //    {
            //        //It's a good idea to reset the DbContext to be shure it dont take cashed records
            //        db2 = new BookShopContext();
            //        author2 = db2.Authors.First(x => x.AuthorId == 1);
            //    }
            //}

            //Exercise
            //var command = Console.ReadLine();
            //var booksTitle = GetBooksByAgeRestriction(db, command);

            //var year = int.Parse(Console.ReadLine());
            //var goldenBooks = GetBooksNotReleasedIn(db, year);

            //var categories = Console.ReadLine();
            //var books = GetBooksByCategory(db, categories);

            //var date = Console.ReadLine();
            //var titles = GetBooksReleasedBefore(db, date);

            //var template = Console.ReadLine();
            //var authors = GetBooksByAuthor(db, template);

            //int length = int.Parse(Console.ReadLine());
            //int count = CountBooks(db, length);

            //IncreasePrices(db);

            RemoveBooks(db);

            //Console.WriteLine(GetMostRecentBooks(db));
        }

        //16. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books.Where(c => c.Copies < 4200).ToList();

            foreach (var item in books)
            {
                var categories = context.BooksCategories.Where(x => x.BookId == item.BookId).ToList();
                context.BooksCategories.RemoveRange(categories);
                context.Books.Remove(item);
            }

            //context.RemoveRange(books);
            context.SaveChanges();

            return books.Count();
        }

        //15. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            context.Books.Where(b => b.ReleaseDate.Value.Year < 2010).ToList().ForEach(b => b.Price += 5);
            context.SaveChanges();
        }

        //14. Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categoriesBooks = context.Categories
                               .Select(c => new
                               {
                                   Name = c.Name,
                                   Books = c.CategoryBooks
                                            //.Where(x => x.Category.Name == c.Name)
                                            .OrderByDescending(b => b.Book.ReleaseDate)
                                            .Take(3)
                                            .Select(b => new
                                            {
                                                Title = b.Book.Title,
                                                Release = b.Book.ReleaseDate.Value.Year
                                            })
                                            .ToList()
                               })
                               .OrderBy(c => c.Name)
                               .ToList();

            var sb = new StringBuilder();

            foreach (var category in categoriesBooks)
            {
                sb.AppendLine($"--{category.Name}");
                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.Release})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //13. Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                                 .Select(c => new
                                 {
                                     Category = c.Name,
                                     Profit = c.CategoryBooks.Select(b => b.Book).Sum(b => b.Price * b.Copies)
                                 })
                                 .OrderByDescending(c => c.Profit)
                                 .ThenBy(c => c.Category)
                                 .ToList();

            var sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"{category.Category} ${category.Profit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //12. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                                .Select(a => new
                                {
                                    FullName = $"{a.FirstName} {a.LastName}",
                                    Copiess = a.Books.Sum(b => b.Copies)
                                })
                                .OrderByDescending(a => a.Copiess)
                                .ToList();

            var sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FullName} - {author.Copiess}");
            }

            return sb.ToString().TrimEnd();
        }

        //11. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                                 .Where(b => b.Title.Length > lengthCheck)
                                 .ToList()
                                 .Count;
        }

        //10. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var authorBooks = context.Authors
                                     .Where(a => a.LastName.ToLower().StartsWith(input.ToLower()))
                                     .SelectMany(a => a.Books)
                                     .OrderBy(b => b.BookId)
                                     .Select(b => new 
                                     {
                                         Title = b.Title,
                                         Author = b.Author.FirstName + " " + b.Author.LastName
                                     })
                                     .ToList();

            var sb = new StringBuilder();

            foreach (var b in authorBooks)
            {
                sb.AppendLine($"{b.Title} ({b.Author})");
            }

            return sb.ToString().TrimEnd();
            //var books = context.Books
            //                   .AsEnumerable()
            //                   .Where(b => b.Author.LastName
            //                                       .StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
            //                   .OrderBy(b => b.BookId)
            //                   .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})")
            //                   .ToList();
            //return string.Join(Environment.NewLine, books);
        }

        //9. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var titles = context.Books
                               .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                               .Select(b => b.Title)
                               .OrderBy(b => b)
                               .ToList();

            var titles2 = context.Books
                               .AsEnumerable()
                               .Where(b => b.Title.Contains(input, StringComparison.InvariantCultureIgnoreCase))
                               .Select(b => b.Title)
                               .OrderBy(b => b)
                               .ToList();

            return string.Join(Environment.NewLine, titles);
        }

        //8. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                                 .Where(a => a.FirstName.ToLower().EndsWith(input.ToLower()))
                                 .OrderBy(a => a.FirstName)
                                 .Select(a => $"{a.FirstName} {a.LastName}")
                                 .ToList();

            return string.Join(Environment.NewLine, authors);
        }

        //7. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var datetime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                                .Where(x => DateTime.Compare(x.ReleaseDate.Value, datetime) < 0)
                                .OrderByDescending(x => x.ReleaseDate)
                                .Select(x => new
                                {
                                    Title = x.Title,
                                    EditionType = x.EditionType.ToString(),
                                    Price = x.Price
                                })
                                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //6. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                  .Select(c => c.ToLower())
                                  .ToArray();
            //EF couldn't translate query
            //var books = context.Categories
            //                   .AsEnumerable()
            //                   .Select(x => x.CategoryBooks
            //                                 .Where(b => categories.Contains(x.Name))
            //                                 .Select(b => new { Title = b.Book.Title }))
            //                   .OrderBy(b => b)
            //                   .ToList();

            //Judge 50/100points, same output
            var books = context.Books
                               .Where(x => x.BookCategories
                                                  .Any(s => categories
                                                            .Contains(s.Category.Name.ToLower())))
                                     .Select(x => x.Title)
                                     .OrderBy(x => x)
                                     .ToList();
            //Judge 100points
            var titles = new List<string>();

            foreach (var category in categories)
            {
                var current = context
                                .Books
                                .Where(b => b.BookCategories.Any(bc => bc.Category.Name.ToLower() == category))
                                .Select(b => b.Title)
                                .ToList();

                titles.AddRange(current);
            }

            var orderedTitles = titles.OrderBy(t => t).ToList();

            return string.Join(Environment.NewLine, orderedTitles);
        }

        //5. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                               .Where(x => x.ReleaseDate.Value.Year != year)
                               .OrderBy(x => x.BookId)
                               .Select(x => x.Title)
                               .ToList();

            return string.Join(Environment.NewLine, books);
        }

        //4. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                               .Where(x => x.Price > 40)
                               .OrderByDescending(x => x.Price)
                               .Select(x => new { x.Title, x.Price })
                               .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //3. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBook = context.Books
                                     .AsEnumerable()
                                     .Where(x => x.EditionType.ToString() == "Gold" && x.Copies < 5000)
                                     .Select(x => new { x.Title, x.BookId })
                                     .OrderBy(x => x.BookId)
                                     .ToList();

            var goldenBooks = context.Books
                                    .Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000)
                                    .OrderBy(x => x.BookId)
                                    .Select(x => x.Title)
                                    .ToList();

            //return string.Join(Environment.NewLine, goldenBook.Select(x => x.Title));
            return string.Join(Environment.NewLine, goldenBooks);
        }

        //2. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var booksTitle = context.Books
                                    .AsEnumerable()
                                    .Where(x => x.AgeRestriction.ToString().ToLower() == command.ToLower())
                                    .Select(x => x.Title)
                                    .OrderBy(x => x)
                                    .ToList();

            var sb = new StringBuilder();

            foreach (var title in booksTitle)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().TrimEnd();
        }
                
    }
}
