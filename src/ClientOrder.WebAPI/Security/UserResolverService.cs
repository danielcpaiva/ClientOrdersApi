using System.Security.Claims;
using ClientOrder.Domain.Constants;
using ClientOrder.Services.Interfaces;

namespace ClientOrder.WebAPI.Security;

public class UserResolverService(IHttpContextAccessor accessor) : IUserResolverService
{
    private readonly ClaimsPrincipal _user = accessor.HttpContext?.User
        ?? throw new InvalidOperationException("There is no HTTP request.");

    public bool IsAuthenticated => _user.Identity?.IsAuthenticated == true;

    public bool IsAdmin =>
        IsAuthenticated && string.Equals(GetRole(), AppRoles.Admin, StringComparison.Ordinal);

    public int GetById()
    {
        if (_user.Identity?.IsAuthenticated != true
            || !int.TryParse(_user.FindFirst("sub")?.Value, out var userId)
            || userId <= 0)
        {
            throw new InvalidOperationException("There is no authenticated user.");
        }

        return userId;
    }

    public string? GetFullName() => _user.FindFirst("name")?.Value;

    public string? GetUserName() => _user.FindFirst("username")?.Value;

    public string? GetRole() => _user.FindFirst("role")?.Value;
}
