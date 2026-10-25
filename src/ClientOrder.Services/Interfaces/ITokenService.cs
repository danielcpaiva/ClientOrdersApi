using ClientOrder.Services.Dtos;

namespace ClientOrder.Services.Interfaces;

public interface ITokenService
{
    TokenDto GenerateToken(UserDto userDto);
}
