using Microsoft.EntityFrameworkCore;
using Stations.Models;
using Stations.Models.Enums;

namespace Stations.Data
{

    public class StationsDbContext : DbContext
    {
        public StationsDbContext()
        {}

        public StationsDbContext(DbContextOptions options)
            : base(options)
        {}

        public DbSet<CustomerCard> CustomerCards { get; set; }
        public DbSet<SeatingClass> SeatingClasses { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<TrainSeat> TrainSeats { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Station>(e =>
            {
                e.HasAlternateKey(x => x.Name);
            });

            builder.Entity<Train>(e =>
            {
                e.HasAlternateKey(x => x.TrainNumber);
            });

            builder.Entity<SeatingClass>(e =>
            {
                e.HasAlternateKey(x => x.Name);
                e.HasAlternateKey(x => x.Abbreviation);
            });

            builder.Entity<TrainSeat>(e =>
            {
                e.HasOne(x => x.SeatingClass)
                .WithMany(s => s.TrainSeats)
                .HasForeignKey(x => x.SeatingClassId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Train)
                .WithMany(t => t.TrainSeats)
                .HasForeignKey(x => x.TrainId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Trip>(e =>
            {
                e.Property(x => x.Status)
                .HasDefaultValue(TripStatus.OnTime);

                e.HasOne(x => x.Train)
                .WithMany(t => t.Trips)
                .HasForeignKey(x => x.TrainId);

                e.HasOne(x => x.OriginStation)
                .WithMany(s => s.TripsFrom)
                .HasForeignKey(x => x.OriginStationId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.DestinationStation)
                .WithMany(s => s.TripsTo)
                .HasForeignKey(x => x.DestinationStationId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Ticket>(e =>
            {
                e.HasOne(x => x.CustomerCard)
                .WithMany(c => c.BoughtTickets)
                .HasForeignKey(x => x.CustomerCardId);
            });

            builder.Entity<CustomerCard>(e =>
            {
                e.Property(x => x.Type)
                .HasDefaultValue(CardType.Normal);
            });
        }
    }
}