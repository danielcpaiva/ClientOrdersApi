using ClientOrder.Services.Dtos;

namespace ClientOrder.Services.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserReadDto>> GetAllAsync();
    Task<UserReadDto> GetByIdAsync(int id);
    Task<UserReadDto> CreateAsync(UserCreateDto userDto);
    Task UpdateAsync(int id, UserCreateEditDto userDto);
    Task DeleteAsync(int id);
    Task ChangeRoleAsync(int id, ChangeRoleDto roleDto);
}
