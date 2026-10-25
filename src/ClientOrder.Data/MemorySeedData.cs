using ClientOrder.Domain.Model;

namespace ClientOrder.Data;

public class SeedUser
{
    public required User User { get; init; }
    public required string Password { get; init; }
}

public class MemorySeedData
{
    public List<SeedUser> Users { get; set; } = [];
}
