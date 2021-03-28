using Microsoft.EntityFrameworkCore;
using PetClinic.Models;

namespace PetClinic.Data
{

    public class PetClinicContext : DbContext
    {
        public PetClinicContext()
        {
        }

        public PetClinicContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Animal> Animals { get; set; }

        public DbSet<AnimalAid> AnimalAids { get; set; }

        public DbSet<Passport> Passports { get; set; }

        public DbSet<Procedure> Procedures { get; set; }

        public DbSet<ProcedureAnimalAid> ProcedureAnimalAids { get; set; }

        public DbSet<Vet> Vets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Animal>(e =>
            {
                e.HasOne(x => x.Passport)
                .WithOne(a => a.Animal)
                .HasForeignKey<Animal>(a => a.PassportSerialNumber)
                .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Vet>(e =>
            {
                e.HasAlternateKey(x => x.PhoneNumber);
            });

            builder.Entity<Procedure>(e =>
            {
                e.HasOne(p => p.Animal)
                .WithMany(a => a.Procedures)
                .HasForeignKey(p => p.AnimalId);

                e.HasOne(p => p.Vet)
                .WithMany(v => v.Procedures)
                .HasForeignKey(p => p.VetId);
            });

            builder.Entity<AnimalAid>(e =>
            {
                e.HasAlternateKey(x => x.Name);
            });

            builder.Entity<ProcedureAnimalAid>(e =>
            {
                e.HasKey(x => new { x.AnimalAidId, x.ProcedureId });

                e.HasOne(p => p.Procedure)
                .WithMany(a => a.ProcedureAnimalAids)
                .HasForeignKey(p => p.ProcedureId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(a => a.AnimalAid)
                .WithMany(p => p.ProcedureAnimalAids)
                .HasForeignKey(a => a.AnimalAidId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}