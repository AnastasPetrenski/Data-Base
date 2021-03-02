using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data.Models;

namespace P01_HospitalDatabase.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext()
        {
        }

        public HospitalContext(DbContextOptions options)
            :base(options)
        {
        }

        public DbSet<Diagnose> Diagnoses { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
        public DbSet<PatientMedicament> PatientMedicaments { get; set; }
        public DbSet<Doctor> Doctor { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Diagnose>(entity => 
            {
                entity
                    .Property(p => p.Name)
                    .IsUnicode(true);

                entity
                    .Property(p => p.Comments)
                    .IsUnicode(true);

                entity
                    .HasOne(p => p.Patient)
                    .WithMany(s => s.Diagnoses)
                    .HasForeignKey(p => p.PatientId);
            });

            modelBuilder.Entity<Medicament>(entity =>
            {
                entity
                    .Property(p => p.Name)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity
                    .Property(p => p.FirstName)
                    .IsUnicode(true);

                entity
                    .Property(p => p.LastName)
                    .IsUnicode(true);

                entity
                    .Property(p => p.Address)
                    .IsUnicode(true);

                entity
                    .Property(p => p.Email)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Visitation>(entity =>
            {
                entity
                    .Property(p => p.Comments)
                    .IsUnicode(true);

                entity
                    .HasOne(p => p.Patient)
                    .WithMany(s => s.Visitations)
                    .HasForeignKey(p => p.PatientId);

                entity
                    .HasOne(p => p.Doctor)
                    .WithMany(d => d.Visitations)
                    .HasForeignKey(p => p.DoctorId);
            });

            modelBuilder.Entity<PatientMedicament>(entity => 
            {
                entity
                    .HasKey(p => new { p.PatientId, p.MedicamentId });

                entity
                    .HasOne(p => p.Patient)
                    .WithMany(s => s.Prescriptions)
                    .HasForeignKey(p => p.PatientId);

                entity
                    .HasOne(p => p.Medicament)
                    .WithMany(s => s.Prescriptions)
                    .HasForeignKey(p => p.MedicamentId);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity
                    .Property(p => p.Name)
                    .IsUnicode(true);

                entity
                    .Property(p => p.Specialty)
                    .IsUnicode(true);
            });
        }
    }
}
