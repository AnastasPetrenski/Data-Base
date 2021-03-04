namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Writer> Writers { get; set; }
        public DbSet<SongPerformer> SongsPerformers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SongPerformer>(entity =>
            {
                entity.HasKey(x => new { x.SongId, x.PerformerId});

                entity
                    .HasOne(s => s.Song)
                    .WithMany(x => x.SongPerformers)
                    .HasForeignKey(s => s.SongId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(s => s.Performer)
                    .WithMany(x => x.PerformerSongs)
                    .HasForeignKey(s => s.PerformerId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            builder.Entity<Song>(entity =>
            {
                entity
                    .HasOne(s => s.Writer)
                    .WithMany(x => x.Songs)
                    .HasForeignKey(s => s.WriterId);

                entity
                    .HasOne(s => s.Album)
                    .WithMany(x => x.Songs)
                    .HasForeignKey(s => s.AlbumId);

            });

            builder.Entity<Album>(entity =>
            {
                entity
                    .HasOne(s => s.Producer)
                    .WithMany(x => x.Albums)
                    .HasForeignKey(s => s.ProducerId);

            });
        }
    }
}
