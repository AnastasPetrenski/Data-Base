namespace Cinema.Data
{
    using Cinema.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class CinemaContext : DbContext
    {
        public CinemaContext()  { }

        public CinemaContext(DbContextOptions options)
            : base(options)   { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Projection> Projections { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Projection>(e =>
            {
                e.HasOne(p => p.Hall)
                .WithMany(h => h.Projections)
                .HasForeignKey(p => p.HallId);

                e.HasOne(p => p.Movie)
               .WithMany(h => h.Projections)
               .HasForeignKey(p => p.MovieId);

            });

            modelBuilder.Entity<Ticket>(e =>
            {
                e.HasOne(p => p.Customer)
               .WithMany(h => h.Tickets)
               .HasForeignKey(p => p.CustomerId);

                e.HasOne(p => p.Projection)
               .WithMany(h => h.Tickets)
               .HasForeignKey(p => p.ProjectionId);

            });

            modelBuilder.Entity<Seat>(e =>
            {
                e.HasOne(p => p.Hall)
               .WithMany(h => h.Seats)
               .HasForeignKey(p => p.HallId);

            });
        }
    }
}