namespace VaporStore.Data
{
    using Microsoft.EntityFrameworkCore;
    using VaporStore.Data.Models;

    public class VaporStoreDbContext : DbContext
    {
        public VaporStoreDbContext()
        {
        }

        public VaporStoreDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameTag> GameTags { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Game>(e =>
            {
                e.HasOne(x => x.Developer)
                .WithMany(d => d.Games)
                .HasForeignKey(x => x.DeveloperId);

                e.HasOne(x => x.Genre)
                .WithMany(d => d.Games)
                .HasForeignKey(x => x.GenreId);
            });

            model.Entity<GameTag>(e =>
            {
                e.HasKey(x => new { x.GameId, x.TagId });


                e.HasOne(x => x.Game)
                .WithMany(d => d.GameTags)
                .HasForeignKey(x => x.GameId);

                e.HasOne(x => x.Tag)
                .WithMany(d => d.GameTags)
                .HasForeignKey(x => x.TagId);

            });

            model.Entity<Card>(e =>
            {
                e.HasOne(x => x.User)
                .WithMany(d => d.Cards)
                .HasForeignKey(x => x.UserId);
            });

            model.Entity<Purchase>(e =>
            {
                e.HasOne(x => x.Card)
                .WithMany(d => d.Purchases)
                .HasForeignKey(x => x.CardId);

                e.HasOne(x => x.Game)
                .WithMany(d => d.Purchases)
                .HasForeignKey(x => x.GameId);
            });
        }
    }
}
