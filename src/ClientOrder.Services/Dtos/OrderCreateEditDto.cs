using ClientOrder.Domain.Model.Enums;

namespace ClientOrder.Services.Dtos;

public class OrderCreateEditDto
{
    public double Amount { get; set; }
    public OrderStatus Status { get; set; }
    public int UserId { get; set; }
}
