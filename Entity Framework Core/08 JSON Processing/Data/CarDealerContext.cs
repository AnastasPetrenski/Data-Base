using CarDealer.Models;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.Data
{
    public class CarDealerContext : DbContext
    {
        public CarDealerContext()
        {
        }

        public CarDealerContext(DbContextOptions options)
            : base(options)
        {
        }


        public DbSet<Car> Cars { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<PartCar> PartCars { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=CarDealer;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartCar>(e =>
            {
                e.HasKey(k => new { k.CarId, k.PartId });

                //e.HasOne(x => x.Car)
                // .WithMany(x => x.PartCars)
                // .HasForeignKey(x => x.CarId)
                // .OnDelete(DeleteBehavior.Restrict);

                //e.HasOne(x => x.Part)
                // .WithMany(x => x.PartCars)
                // .HasForeignKey(x => x.CarId)
                // .OnDelete(DeleteBehavior.Restrict);
            });

            //modelBuilder.Entity<Part>(e =>
            //{
            //    e.HasOne(x => x.Supplier)
            //     .WithMany(x => x.Parts)
            //     .HasForeignKey(x => x.SupplierId);

            //});

            //modelBuilder.Entity<Sale>(e =>
            //{
            //    e.HasOne(x => x.Customer)
            //     .WithMany(x => x.Sales)
            //     .HasForeignKey(x => x.CustomerId);

            //    e.HasOne(x => x.Car)
            //     .WithMany(x => x.Sales)
            //     .HasForeignKey(x => x.CarId);
            //});
        }
    }
}
