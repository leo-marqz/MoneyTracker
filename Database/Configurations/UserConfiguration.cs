
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyTracker.Models;

namespace MoneyTracker.Database.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property((user) => user.Firstname).HasMaxLength(50).IsRequired();
            builder.Property((user) => user.Lastname).HasMaxLength(50).IsRequired();
            builder.Property((user) => user.RefreshToken).IsRequired(false);
            builder.Property((user) => user.RefreshTokenExpiry);
            builder.Property((user) => user.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property((user) => user.LastModifiedAt).HasDefaultValueSql("GETDATE()");
            builder.Property((user) => user.IsDeleted).HasDefaultValue(false);
        }
    }
}