using ClientOrder.Domain.Model.Enums;

namespace ClientOrder.Services.Dtos;

public class OrderReadDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public double Amount { get; set; }
    public OrderStatus Status { get; set; }
    public int UserId { get; set; }
}
