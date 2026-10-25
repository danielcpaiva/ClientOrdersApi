using ClientOrder.Data;
using ClientOrder.Services.Implementations;
using ClientOrder.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClientOrder.IoC;

public static class DependencyInjection
{
    public static void AddClientOrderServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAccountRoleService, AccountRoleService>();

        var connectionString = configuration.GetConnectionString("ClientOrder");

        services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }

    public static void Seed(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

        if (!context.Users.Any())
        {
            context.SeedData(passwordService.HashPassword);
            context.SaveChanges();
        }
    }
}
