using ClientOrder.Domain.Constants;
using ClientOrder.Domain.Model;

namespace ClientOrder.Data;

public static class MemoryDomainSeeder
{
    public static MemorySeedData CreateSeedData()
    {
        var seedData = new MemorySeedData();

        AddUser(seedData, "Admin", "User", 40, "ADMIN0001", "admin", "Admin123!", AppRoles.Admin);
        AddUser(seedData, "Ana", "Silva", 28, "ANA00001", "ana", "Ana12345!", AppRoles.User);
        AddUser(seedData, "Pedro", "Costa", 32, "PEDRO0001", "pedro", "Pedro123!", AppRoles.User);
        return seedData;
    }

    private static void AddUser(
        MemorySeedData seedData,
        string firstName,
        string lastName,
        int age,
        string idCard,
        string username,
        string password,
        string role)
    {
        var user = new User(firstName, lastName, age, idCard, username, password, role);
        seedData.Users.Add(new SeedUser
        {
            User = user,
            Password = password
        });
    }
}
