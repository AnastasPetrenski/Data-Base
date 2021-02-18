using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace EF_03_Intro.Geography
{
    public partial class GeographyContext : DbContext
    {
        public GeographyContext()
        {
        }

        public GeographyContext(DbContextOptions<GeographyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Continent> Continents { get; set; }
        public virtual DbSet<CountriesRiver> CountriesRivers { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<FinalTable> FinalTables { get; set; }
        public virtual DbSet<Info> Infos { get; set; }
        public virtual DbSet<Mountain> Mountains { get; set; }
        public virtual DbSet<MountainsCountry> MountainsCountries { get; set; }
        public virtual DbSet<Peak> Peaks { get; set; }
        public virtual DbSet<River> Rivers { get; set; }
        public virtual DbSet<VAbove8000Meter> VAbove8000Meters { get; set; }
        public virtual DbSet<VHighestPeak> VHighestPeaks { get; set; }
        public virtual DbSet<VPeaksBetween5000And6000Meter> VPeaksBetween5000And6000Meters { get; set; }
        public virtual DbSet<VPeaksBetween6000And7000Meter> VPeaksBetween6000And7000Meters { get; set; }
        public virtual DbSet<VPeaksBetween7000And8000Meter> VPeaksBetween7000And8000Meters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=Geography;Integrated Security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Continent>(entity =>
            {
                entity.Property(e => e.ContinentCode)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<CountriesRiver>(entity =>
            {
                entity.HasKey(e => new { e.CountryCode, e.RiverId });

                entity.Property(e => e.CountryCode)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.CountryCodeNavigation)
                    .WithMany(p => p.CountriesRivers)
                    .HasForeignKey(d => d.CountryCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountriesRivers_Countries");

                entity.HasOne(d => d.River)
                    .WithMany(p => p.CountriesRivers)
                    .HasForeignKey(d => d.RiverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountriesRivers_Rivers");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.CountryCode)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Capital).IsUnicode(false);

                entity.Property(e => e.ContinentCode)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CountryName).IsUnicode(false);

                entity.Property(e => e.CurrencyCode)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IsoCode)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.ContinentCodeNavigation)
                    .WithMany(p => p.Countries)
                    .HasForeignKey(d => d.ContinentCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Countries_Continents");

                entity.HasOne(d => d.CurrencyCodeNavigation)
                    .WithMany(p => p.Countries)
                    .HasForeignKey(d => d.CurrencyCode)
                    .HasConstraintName("FK_Countries_Currencies");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.Property(e => e.CurrencyCode)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Info>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<MountainsCountry>(entity =>
            {
                entity.HasKey(e => new { e.MountainId, e.CountryCode })
                    .HasName("PK_MountainsContinents");

                entity.Property(e => e.CountryCode)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.CountryCodeNavigation)
                    .WithMany(p => p.MountainsCountries)
                    .HasForeignKey(d => d.CountryCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MountainsCountries_Countries");

                entity.HasOne(d => d.Mountain)
                    .WithMany(p => p.MountainsCountries)
                    .HasForeignKey(d => d.MountainId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MountainsCountries_Mountains");
            });

            modelBuilder.Entity<Peak>(entity =>
            {
                entity.HasOne(d => d.Mountain)
                    .WithMany(p => p.Peaks)
                    .HasForeignKey(d => d.MountainId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Peaks_Mountains");
            });

            modelBuilder.Entity<River>(entity =>
            {
                entity.HasOne(d => d.Mountain)
                    .WithMany(p => p.Rivers)
                    .HasForeignKey(d => d.MountainId)
                    .HasConstraintName("FK__Rivers__Spring__5CD6CB2B");
            });

            modelBuilder.Entity<VAbove8000Meter>(entity =>
            {
                entity.ToView("V_Above_8000_meters");
            });

            modelBuilder.Entity<VHighestPeak>(entity =>
            {
                entity.ToView("V_HighestPeak");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<VPeaksBetween5000And6000Meter>(entity =>
            {
                entity.ToView("V_Peaks_Between_5000_AND_6000_meters");
            });

            modelBuilder.Entity<VPeaksBetween6000And7000Meter>(entity =>
            {
                entity.ToView("V_Peaks_Between_6000_And_7000_meters");
            });

            modelBuilder.Entity<VPeaksBetween7000And8000Meter>(entity =>
            {
                entity.ToView("V_Peaks_Between_7000_And_8000_meters");
            });

            modelBuilder.HasSequence<int>("seq_Numbers")
                .StartsAt(0)
                .IncrementsBy(101);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
