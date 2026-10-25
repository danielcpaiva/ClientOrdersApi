using ClientOrder.Domain.Constants;
using ClientOrder.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientOrder.Data.Configurations;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(account => account.Id);

        builder.Property(account => account.Username)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(account => account.Username)
            .IsUnique();

        builder.Property(account => account.PasswordHash)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(account => account.Role)
            .HasMaxLength(20)
            .HasDefaultValue(AppRoles.User)
            .IsRequired();
    }
}
