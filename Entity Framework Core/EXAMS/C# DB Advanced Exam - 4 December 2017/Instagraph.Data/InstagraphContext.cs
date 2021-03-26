using Instagraph.Models;
using Microsoft.EntityFrameworkCore;

namespace Instagraph.Data
{
    public class InstagraphContext : DbContext
    {
        public InstagraphContext()
        {
        }

        public InstagraphContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFollower> UsersFollowers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(e =>
            {
                e.HasAlternateKey(x => x.Username);

                e.HasOne(x => x.ProfilePicture)
                .WithMany(p => p.Users)
                .HasForeignKey(x => x.ProfilePictureId);
            });

            builder.Entity<Post>(e =>
            {
                e.HasOne(x => x.Picture)
                .WithMany(p => p.Posts)
                .HasForeignKey(x => x.PictureId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Comment>(e =>
            {
                e.HasOne(x => x.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(x => x.UserId);

                e.HasOne(x => x.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(x => x.PostId);
            });

            builder.Entity<UserFollower>(e =>
            {
                e.HasKey(x => new { x.FollowerId, x.UserId });

                e.HasOne(x => x.User)
                .WithMany(u => u.UsersFollowing)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Follower)
                .WithMany(u => u.Followers)
                .HasForeignKey(x => x.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}