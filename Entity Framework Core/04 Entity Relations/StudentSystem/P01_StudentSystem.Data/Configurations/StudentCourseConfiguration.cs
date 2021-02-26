using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data.Configurations
{
    public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
    {
        public void Configure(EntityTypeBuilder<StudentCourse> builder)
        {
            builder.HasKey(s => new { s.CourseId, s.StudentId });

            builder
                .HasOne(s => s.Student)
                .WithMany(c => c.CourseEnrollments)
                .HasForeignKey(s => s.StudentId);

            builder
                .HasOne(c => c.Course)
                .WithMany(s => s.StudentsEnrolled)
                .HasForeignKey(c => c.CourseId);
        }
    }
}
