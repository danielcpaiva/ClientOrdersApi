using ClientOrder.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientOrder.Data.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.FirstName)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(user => user.LastName)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(user => user.IdCard)
            .HasMaxLength(32)
            .IsRequired();

        builder.HasMany(user => user.Orders)
            .WithOne(order => order.User)
            .HasForeignKey(order => order.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(user => user.Account)
            .WithOne(account => account.User)
            .HasForeignKey<Account>(account => account.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(user => user.Account)
            .IsRequired();
    }
}
