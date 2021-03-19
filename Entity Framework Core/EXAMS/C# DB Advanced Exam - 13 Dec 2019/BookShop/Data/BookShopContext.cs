namespace BookShop.Data
{
    using BookShop.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class BookShopContext : DbContext
    {
        public BookShopContext() { }

        public BookShopContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorBook> AuthorsBooks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity
                    .Property(a => a.FirstName)
                    .IsRequired(true)
                    .HasMaxLength(30);

                entity
                    .Property(a => a.LastName)
                    .IsRequired(true)
                    .HasMaxLength(30);

                entity
                    .Property(e => e.Email)
                    .IsRequired(true);

                entity
                    .Property(p => p.Phone)
                    .IsRequired(true)
                    .HasMaxLength(12);

            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity
                    .Property(b => b.Name)
                    .IsRequired(true)
                    .HasMaxLength(30);

                entity
                    .Property(b => b.Genre)
                    .IsRequired(true);

                entity
                    .Property(b => b.PublishedOn)
                    .IsRequired(true);

            });

            modelBuilder.Entity<AuthorBook>(entity =>
            {
                entity.HasKey(ab => new { ab.AuthorId, ab.BookId });

                entity
                    .HasOne(a => a.Author)
                    .WithMany(a => a.AuthorsBooks)
                    .HasForeignKey(a => a.AuthorId);

                entity
                    .HasOne(b => b.Book)
                    .WithMany(b => b.AuthorsBooks)
                    .HasForeignKey(b => b.BookId);
            });
        }
    }
}