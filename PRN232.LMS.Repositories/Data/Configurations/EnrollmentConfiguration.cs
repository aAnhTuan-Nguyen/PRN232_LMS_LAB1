using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollment");

        builder.HasKey(enrollment => enrollment.EnrollmentId);

        builder.Property(enrollment => enrollment.EnrollDate)
            .IsRequired();

        builder.Property(enrollment => enrollment.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(enrollment => enrollment.Student)
            .WithMany(student => student.Enrollments)
            .HasForeignKey(enrollment => enrollment.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(enrollment => enrollment.Course)
            .WithMany(course => course.Enrollments)
            .HasForeignKey(enrollment => enrollment.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(enrollment => enrollment.StudentId);
        builder.HasIndex(enrollment => enrollment.CourseId);
        builder.HasIndex(enrollment => enrollment.Status);
    }
}
