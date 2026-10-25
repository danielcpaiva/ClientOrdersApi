namespace ClientOrder.Services.Dtos;

public class UserCreateEditDto
{
    public int Age { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string IdCard { get; set; } = string.Empty;
}
