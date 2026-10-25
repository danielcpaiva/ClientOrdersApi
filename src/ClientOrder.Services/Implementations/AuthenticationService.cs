using ClientOrder.Data;
using ClientOrder.Domain.Constants;
using ClientOrder.Services.Dtos;
using ClientOrder.Services.Exceptions;
using ClientOrder.Services.Interfaces;
using ClientOrder.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ClientOrder.Services.Implementations;

public class AuthenticationService(ApplicationContext context, IPasswordService passwordService, IUserService userService)
    : IAuthenticationService
{
    public async Task<UserDto> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidCredentialsException("Invalid credentials.");
        }

        var normalizedUsername = username.Trim().ToLower();
        var user = await context.Users
            .Include(existingUser => existingUser.Account)
            .SingleOrDefaultAsync(existingUser => existingUser.Account.Username == normalizedUsername);

        if (user is null || !passwordService.VerifyPassword(user.Account, password))
        {
            throw new InvalidCredentialsException("Invalid credentials.");
        }

        user.Account.PasswordHash = passwordService.HashPassword(user.Account, password);
        await context.SaveChangesAsync();

        return DtoMapper.ToUserDto(user);
    }

    public async Task<UserDto> RegisterAsync(RegisterUserDto userDto)
    {
        if (userDto is null)
        {
            throw new ServiceException("User data is required.");
        }

        var created = await userService.CreateAsync(new UserCreateDto
        {
            Age = userDto.Age,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            IdCard = userDto.IdCard,
            Username = userDto.Username,
            Password = userDto.Password
        });

        return new UserDto
        {
            Id = created.Id,
            FullName = $"{created.FirstName} {created.LastName}".Trim(),
            Username = userDto.Username.Trim().ToLower(),
            Role = AppRoles.User
        };
    }
}
