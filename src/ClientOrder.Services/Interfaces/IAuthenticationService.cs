using ClientOrder.Services.Dtos;

namespace ClientOrder.Services.Interfaces;

public interface IAuthenticationService
{
    Task<UserDto> LoginAsync(string username, string password);
    Task<UserDto> RegisterAsync(RegisterUserDto userDto);
}
