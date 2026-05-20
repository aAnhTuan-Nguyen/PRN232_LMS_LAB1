using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Course");

        builder.HasKey(course => course.CourseId);

        builder.Property(course => course.CourseName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(course => course.Semester)
            .WithMany(semester => semester.Courses)
            .HasForeignKey(course => course.SemesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(course => course.Subject)
            .WithMany(subject => subject.Courses)
            .HasForeignKey(course => course.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(course => course.SemesterId);
        builder.HasIndex(course => course.SubjectId);
    }
}
