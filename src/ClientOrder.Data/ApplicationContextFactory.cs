using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ClientOrder.Data;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>

{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var settingsPath = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "ClientOrder.WebAPI",
            "appsettings.json"));

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(settingsPath, optional: false)
            .Build(); 

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("ClientOrder"));

        return new ApplicationContext(optionsBuilder.Options);
    }
}
