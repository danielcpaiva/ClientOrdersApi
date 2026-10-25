using ClientOrder.Domain.Model;
using ClientOrder.Services.Dtos;

namespace ClientOrder.Services.Mappers;

internal static class DtoMapper
{
    public static UserReadDto ToDto(User user)
    {
        return new UserReadDto
        {
            Id = user.Id,
            Age = user.Age,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IdCard = user.IdCard,
            Orders = user.Orders
                .Select(ToDto)
                .ToList()
        };
    }

    public static UserDto ToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Username = user.Account.Username,
            Role = user.Account.Role
        };
    }

    public static OrderReadDto ToDto(Order order)
    {
        return new OrderReadDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            Amount = order.Amount,
            Status = order.Status,
            UserId = order.UserId
        };
    }
}
