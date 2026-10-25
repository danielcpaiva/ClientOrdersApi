using ClientOrder.Domain.Model;

namespace ClientOrder.Services.Interfaces;

public interface IPasswordService
{
    string HashPassword(Account account, string password);
    bool VerifyPassword(Account account, string password);
}
