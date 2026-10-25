namespace ClientOrder.Services.Interfaces;

public interface IAccountRoleService
{
    Task<string?> GetRoleAsync(int userId);
}
