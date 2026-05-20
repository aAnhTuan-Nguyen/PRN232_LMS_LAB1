using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Student");

        builder.HasKey(student => student.StudentId);

        builder.Property(student => student.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(student => student.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(student => student.DateOfBirth)
            .IsRequired();

        builder.HasIndex(student => student.Email)
            .IsUnique();
    }
}
