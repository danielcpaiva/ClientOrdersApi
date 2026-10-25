namespace ClientOrder.Services.Interfaces;

public interface IUserResolverService
{
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    int GetById();
    string? GetFullName();
    string? GetUserName();
    string? GetRole();
}
