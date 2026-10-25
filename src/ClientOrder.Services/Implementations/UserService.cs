using ClientOrder.Data;
using ClientOrder.Domain.Constants;
using ClientOrder.Domain.Model;
using ClientOrder.Services.Dtos;
using ClientOrder.Services.Exceptions;
using ClientOrder.Services.Interfaces;
using ClientOrder.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ClientOrder.Services.Implementations;

public class UserService : IUserService
{
    private readonly ApplicationContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IUserResolverService _userResolver;

    public UserService(
        ApplicationContext context,
        IPasswordService passwordService,
        IUserResolverService userResolver)
    {
        _context = context;
        _passwordService = passwordService;
        _userResolver = userResolver;
    }

    public async Task<IReadOnlyList<UserReadDto>> GetAllAsync()
    {
        var query = _context.Users
            .Include(user => user.Orders)
            .AsNoTracking();

        if (!_userResolver.IsAdmin)
        {
            var currentUserId = _userResolver.GetById();
            query = query.Where(user => user.Id == currentUserId);
        }

        var users = await query
            .OrderBy(user => user.Id)
            .ToListAsync();

        return users.Select(DtoMapper.ToDto).ToList();
    }

    public async Task<UserReadDto> GetByIdAsync(int id)
    {
        ValidatePositiveId(id, "User ID must be greater than zero.");
        EnsureOwnUser(id);

        var user = await _context.Users
            .Include(existingUser => existingUser.Orders)
            .AsNoTracking()
            .SingleOrDefaultAsync(existingUser => existingUser.Id == id);

        if (user is null)
        {
            throw new ServiceNotFoundException("User was not found.");
        }

        return DtoMapper.ToDto(user);
    }

    public async Task<UserReadDto> CreateAsync(UserCreateDto userDto)
    {
        if (_userResolver.IsAuthenticated)
        {
            EnsureAdmin();
        }

        ValidateProfile(userDto);

        var user = new User(
            userDto.FirstName,
            userDto.LastName,
            userDto.Age,
            userDto.IdCard,
            userDto.Username,
            userDto.Password,
            AppRoles.User);

        var usernameTaken = await _context.Accounts
            .AsNoTracking()
            .AnyAsync(account => account.Username == user.Account.Username);

        if (usernameTaken)
        {
            throw new ServiceException("Username already exists.");
        }

        user.Account.PasswordHash = _passwordService.HashPassword(user.Account, userDto.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return DtoMapper.ToDto(user);
    }

    public async Task UpdateAsync(int id, UserCreateEditDto userDto)
    {
        ValidatePositiveId(id, "User ID must be greater than zero.");
        ValidateProfile(userDto);

        var user = await _context.Users
            .SingleOrDefaultAsync(existingUser => existingUser.Id == id);

        if (user is null)
        {
            throw new ServiceNotFoundException("User was not found.");
        }

        EnsureOwnUser(user.Id);

        user.Age = userDto.Age;
        user.FirstName = userDto.FirstName.Trim();
        user.LastName = userDto.LastName.Trim();
        user.IdCard = userDto.IdCard.Trim();

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        ValidatePositiveId(id, "User ID must be greater than zero.");

        var user = await _context.Users
            .SingleOrDefaultAsync(existingUser => existingUser.Id == id);

        if (user is null)
        {
            throw new ServiceNotFoundException("User was not found.");
        }

        EnsureOwnUser(user.Id);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task ChangeRoleAsync(int id, ChangeRoleDto roleDto)
    {
        EnsureAdmin();
        ValidatePositiveId(id, "User ID must be greater than zero.");

        if (roleDto is null || string.IsNullOrWhiteSpace(roleDto.Role))
        {
            throw new ServiceException("Role is required.");
        }

        if (roleDto.Role != AppRoles.User && roleDto.Role != AppRoles.Admin)
        {
            throw new ServiceException("Role is invalid.");
        }

        var user = await _context.Users
            .Include(existingUser => existingUser.Account)
            .SingleOrDefaultAsync(existingUser => existingUser.Id == id);

        if (user is null)
        {
            throw new ServiceNotFoundException("User was not found.");
        }

        user.Account.ChangeRole(roleDto.Role);
        await _context.SaveChangesAsync();
    }

    private static void ValidateProfile(UserCreateEditDto userDto)
    {
        if (userDto is null)
        {
            throw new ServiceException("User data is required.");
        }

        if (userDto.Age < 0)
        {
            throw new ServiceException("Age must be zero or greater.");
        }

        if (string.IsNullOrWhiteSpace(userDto.FirstName))
        {
            throw new ServiceException("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(userDto.LastName))
        {
            throw new ServiceException("Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(userDto.IdCard))
        {
            throw new ServiceException("Id card is required.");
        }
    }

    private static void ValidateProfile(UserCreateDto userDto)
    {
        if (userDto is null)
        {
            throw new ServiceException("User data is required.");
        }

        ValidateProfile(new UserCreateEditDto
        {
            Age = userDto.Age,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            IdCard = userDto.IdCard
        });

        if (string.IsNullOrWhiteSpace(userDto.Username))
        {
            throw new ServiceException("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(userDto.Password))
        {
            throw new ServiceException("Password is required.");
        }
    }

    private static void ValidatePositiveId(int id, string message)
    {
        if (id <= 0)
        {
            throw new ServiceException(message);
        }
    }

    private void EnsureAdmin()
    {
        if (!_userResolver.IsAdmin)
        {
            throw new ServiceForbiddenException("Only an administrator can perform this action.");
        }
    }

    private void EnsureOwnUser(int userId)
    {
        if (_userResolver.IsAdmin)
        {
            return;
        }

        if (_userResolver.GetById() != userId)
        {
            throw new ServiceForbiddenException("You can only access your own data.");
        }
    }
}
