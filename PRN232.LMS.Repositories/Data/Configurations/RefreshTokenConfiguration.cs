using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken");

        builder.HasKey(refreshToken => refreshToken.RefreshTokenId);

        builder.Property(refreshToken => refreshToken.Token)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.ExpiresAt)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.CreatedAt)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.ReplacedByToken)
            .HasMaxLength(256);

        builder.Ignore(refreshToken => refreshToken.IsExpired);
        builder.Ignore(refreshToken => refreshToken.IsRevoked);
        builder.Ignore(refreshToken => refreshToken.IsActive);

        builder.HasOne(refreshToken => refreshToken.User)
            .WithMany(user => user.RefreshTokens)
            .HasForeignKey(refreshToken => refreshToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(refreshToken => refreshToken.Token)
            .IsUnique();

        builder.HasIndex(refreshToken => refreshToken.UserId);
    }
}
