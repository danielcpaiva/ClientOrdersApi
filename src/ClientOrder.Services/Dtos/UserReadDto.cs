namespace ClientOrder.Services.Dtos;

public class UserReadDto
{
    public int Id { get; set; }
    public int Age { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string IdCard { get; set; } = string.Empty;
    public IReadOnlyList<OrderReadDto> Orders { get; set; } = [];
}
