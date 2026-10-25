using ClientOrder.Domain.Constants;
using ClientOrder.Domain.Exceptions;
using ClientOrder.Domain.Model.Common;

namespace ClientOrder.Domain.Model;

public class Account : Entity
{
    private const int MinimumPasswordLength = 8;
    private const string SpecialCharacters = "!@#$%^&*()-_=+[]{};:,.<>?/";

    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = AppRoles.User;
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    private Account()
    {
    }

    public Account(string username, string password, string role) : this()
    {
        ChangeUsername(username);
        ValidatePassword(password);
        ChangeRole(role);
    }

    public void ChangeUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new InvalidAccountException("Username is required.");
        }

        var normalizedUsername = username.Trim().ToLower();

        if (normalizedUsername.Length < 3)
        {
            throw new InvalidAccountException("Username must be at least 3 characters long.");
        }

        if (normalizedUsername.Length > 20)
        {
            throw new InvalidAccountException("Username must be at most 20 characters long.");
        }

        if (normalizedUsername.Contains(' '))
        {
            throw new InvalidAccountException("Username cannot contain spaces.");
        }

        foreach (var character in normalizedUsername)
        {
            if (!char.IsLetterOrDigit(character))
            {
                throw new InvalidAccountException("Username can only contain letters and numbers.");
            }
        }

        Username = normalizedUsername;
    }

    public void ChangeRole(string role)
    {
        if (role != AppRoles.User && role != AppRoles.Admin)
        {
            throw new InvalidAccountException("Role is invalid.");
        }

        Role = role;
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidAccountException("Password is required.");
        }

        if (password.Length < MinimumPasswordLength)
        {
            throw new InvalidAccountException("Password must have at least 8 characters.");
        }

        if (!password.Any(char.IsUpper))
        {
            throw new InvalidAccountException("Password must contain at least one capital letter.");
        }

        if (!password.Any(char.IsLower))
        {
            throw new InvalidAccountException("Password must contain at least one lowercase letter.");
        }

        if (!password.Any(char.IsDigit))
        {
            throw new InvalidAccountException("Password must contain at least one number.");
        }

        if (!password.Any(character => SpecialCharacters.Contains(character)))
        {
            throw new InvalidAccountException("Password must contain at least one special character.");
        }
    }
}
