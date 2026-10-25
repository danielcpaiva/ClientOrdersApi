using ClientOrder.Domain.Model;
using ClientOrder.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ClientOrder.Services.Implementations;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<Account> _hasher = new();

    public string HashPassword(Account account, string password)
    {
        return _hasher.HashPassword(account, password);
    }

    public bool VerifyPassword(Account account, string password)
    {
        if (string.IsNullOrWhiteSpace(account.PasswordHash))
        {
            return false;
        }

        var result = _hasher.VerifyHashedPassword(account, account.PasswordHash, password);
        return result != PasswordVerificationResult.Failed;
    }
}
