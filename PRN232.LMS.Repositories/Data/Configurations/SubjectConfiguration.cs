using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subject");

        builder.HasKey(subject => subject.SubjectId);

        builder.Property(subject => subject.SubjectCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(subject => subject.SubjectName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(subject => subject.Credit)
            .IsRequired();

        builder.HasIndex(subject => subject.SubjectCode)
            .IsUnique();
    }
}
