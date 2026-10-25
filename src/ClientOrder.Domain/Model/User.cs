using ClientOrder.Domain.Exceptions;
using ClientOrder.Domain.Model.Common;

namespace ClientOrder.Domain.Model;

public class User : Entity
{
    public int Age { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string IdCard { get; set; } = string.Empty;
    public Account Account { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public User()
    {
    }

    public User(string firstName, string lastName, int age, string idCard, string username, string password, string role)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new InvalidUserException("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new InvalidUserException("Last name is required.");
        }

        if (age < 0)
        {
            throw new InvalidUserException("Age must be zero or greater.");
        }

        if (string.IsNullOrWhiteSpace(idCard))
        {
            throw new InvalidUserException("Id card is required.");
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Age = age;
        IdCard = idCard.Trim();
        Account = new Account(username, password, role);
    }
}
