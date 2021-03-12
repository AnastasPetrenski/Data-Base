using Microsoft.EntityFrameworkCore;
using ProductShop.Models;

namespace ProductShop.Data
{
    public class ProductShopContext : DbContext
    {
        public ProductShopContext()
        {}

        public ProductShopContext(DbContextOptions options)
            :base(options)
        {}

        //TODO: add properties DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                entity.HasKey(cp => new { cp.CategoryId, cp.ProductId });

                entity
                    .HasOne(cp => cp.Category)
                    .WithMany(c => c.CategoryProducts)
                    .HasForeignKey(cp => cp.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(cp => cp.Product)
                    .WithMany(p => p.CategoryProducts)
                    .HasForeignKey(cp => cp.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity
                    .HasOne(p => p.Buyer)
                    .WithMany(u => u.ProductsBought)
                    .HasForeignKey(p => p.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(p => p.Seller)
                    .WithMany(u => u.ProductsSold)
                    .HasForeignKey(p => p.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
