namespace BookShop
{
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            var bookId = 4;
            var songs = db.Books.FromSqlInterpolated($"SELECT * FROM Books Where BookId <= {bookId}").ToList();

            //Take all information
            var minPrice = 15;
            var books = db.Books.FromSqlRaw($"SELECT * FROM Books WHERE Price > {minPrice}").ToList();

            //Eager loading == .Include()
            var selectedBooks = db.Books.Include(x => x.BookCategories).Where(x => x.Price > 15).ToList();

            //Stop tracking
            var cur = db.Books.AsNoTracking().Where(x => x.Author.AuthorId == 1).ToList();

            //Anonymous object or Classes which are unknown for DbContext, ChangeTracker stop tracking changes  
            var num = db.Books.Select(x => new
            {
                Name = x.Author.Books.Where(b => b.Title.Length > 3)
            }).ToList();

            //Z.EntityFramework.Plus Library
            //var deleted = db.Authors.Where(x => x.Books.Count <= 1).Delete();

            foreach (var book in books)
            {
                //Explicit loading, before that Author is null
                db.Entry(book).Reference(x => x.Author).Load();
                //Stop tracking entity changes
                db.Entry(book).State = EntityState.Detached;

                System.Console.WriteLine($"{book.Author.FirstName + " " + book.Author.LastName} {book.Title} {book.Price}");
                System.Console.WriteLine($"{string.Join(" ", book.BookCategories)}");
            }

            //N+1 Problem test with Lazy Loading
            var allBooksBelowFifty = db.Books
                                       .Where(x => x.BookId <= 50)
                                       .ToList();

            foreach (var item in allBooksBelowFifty)
            {
                System.Console.WriteLine($"{item.Title} + {item.Author.Books.Count}");
            }

            //Preventing N+1 Problem is using Select projection 
            var allBooksBelowFiftyProjection = db.Books
                                       .Where(x => x.BookId <= 50)
                                       .Select(x => new { x.Title, x.Author.Books.Count})
                                       .ToList();

            foreach (var item in allBooksBelowFiftyProjection)
            {
                System.Console.WriteLine(item.Title + " " + item.Count);
            }

        }
    }
}
