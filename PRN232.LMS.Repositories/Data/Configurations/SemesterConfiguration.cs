using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data.Configurations;

public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        builder.ToTable("Semester");

        builder.HasKey(semester => semester.SemesterId);

        builder.Property(semester => semester.SemesterName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(semester => semester.StartDate)
            .IsRequired();

        builder.Property(semester => semester.EndDate)
            .IsRequired();
    }
}
