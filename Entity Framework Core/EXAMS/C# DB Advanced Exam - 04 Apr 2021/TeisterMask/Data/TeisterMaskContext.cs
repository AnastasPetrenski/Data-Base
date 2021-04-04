﻿namespace TeisterMask.Data
{
    using Microsoft.EntityFrameworkCore;
    using TeisterMask.Data.Models;

    public class TeisterMaskContext : DbContext
    {
        public TeisterMaskContext() { }

        public TeisterMaskContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeTask> EmployeesTasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }

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
            modelBuilder.Entity<Task>(e =>
            {
                e.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId);
            });

            modelBuilder.Entity<EmployeeTask>(e =>
            {
                e.HasKey(et => new { et.EmployeeId, et.TaskId });

                e.HasOne(et => et.Employee)
                .WithMany(m => m.EmployeesTasks)
                .HasForeignKey(et => et.EmployeeId);

                e.HasOne(et => et.Task)
                .WithMany(t => t.EmployeesTasks)
                .HasForeignKey(et => et.TaskId);
            });
        }
    }
}
