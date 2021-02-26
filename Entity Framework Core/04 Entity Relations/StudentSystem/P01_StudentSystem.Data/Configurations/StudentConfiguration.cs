using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.StudentId);

            builder
                .Property(s => s.Name)
                .IsRequired(true)
                .HasMaxLength(100)
                .IsUnicode(true);

            builder
                .Property(s => s.PhoneNumber)
                .IsRequired(false)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength(true);

            builder
                .Property(c => c.RegisteredOn)
                .IsRequired(true);

            builder
                .Property(s => s.Birthday)
                .IsRequired(false);

        }
    }
}
