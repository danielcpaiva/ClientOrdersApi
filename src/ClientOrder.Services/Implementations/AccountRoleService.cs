using ClientOrder.Data;
using ClientOrder.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClientOrder.Services.Implementations;

public class AccountRoleService(ApplicationContext context) : IAccountRoleService
{
    public Task<string?> GetRoleAsync(int userId)
    {
        return context.Accounts
            .AsNoTracking()
            .Where(account => account.UserId == userId)
            .Select(account => account.Role)
            .SingleOrDefaultAsync();
    }
}
