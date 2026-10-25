using ClientOrder.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace ClientOrder.Data;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
    }

    public void SeedData(Func<Account, string, string> hashPassword)
    {
        var seedData = MemoryDomainSeeder.CreateSeedData();

        foreach (var entry in seedData.Users)
        {
            entry.User.Account.PasswordHash = hashPassword(entry.User.Account, entry.Password);
            Users.Add(entry.User);
        }
    }
}
