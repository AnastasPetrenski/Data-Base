using Microsoft.EntityFrameworkCore;
using P03_SalesDatabase.Data.Models;

namespace P03_SalesDatabase.Data
{
    public class SalesContext : DbContext
    {
        public SalesContext()
        {
        }

        public SalesContext(DbContextOptions options)
            :base(options)
        {
        }

        //DOTO: DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Store> Stores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.CustomerId);

                entity
                    .Property(c => c.Name)
                    .IsRequired(true)
                    .IsUnicode(true)
                    .HasMaxLength(100);

                entity
                    .Property(c => c.Email)
                    .IsRequired(true)
                    .IsUnicode(false)
                    .HasMaxLength(80);

                entity
                    .Property(c => c.CreditCardNumber)
                    .IsRequired(true);

            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);

                entity
                    .Property(c => c.Name)
                    .IsRequired(true)
                    .IsUnicode(true)
                    .HasMaxLength(50);

                entity
                    .Property(c => c.Quantity)
                    .IsRequired(true);

                entity
                    .Property(c => c.Price)
                    .IsRequired(true);

                entity
                    .Property(c => c.Description)
                    .HasMaxLength(250)
                    .HasDefaultValue("No description");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(s => s.StoreId);

                entity
                    .Property(c => c.Name)
                    .IsRequired(true)
                    .IsUnicode(true)
                    .HasMaxLength(80);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(s => s.SaleId);

                entity
                    .Property(c => c.Date)
                    .IsRequired(true)
                    .HasDefaultValueSql("GETDATE()");

                entity
                    .HasOne(s => s.Customer)
                    .WithMany(x => x.Sales)
                    .HasForeignKey(s => s.CustomerId);

                entity
                    .HasOne(s => s.Product)
                    .WithMany(x => x.Sales)
                    .HasForeignKey(s => s.ProductId);

                entity
                    .HasOne(s => s.Store)
                    .WithMany(x => x.Sales)
                    .HasForeignKey(s => s.StoreId);
            });
        }
    }
}
