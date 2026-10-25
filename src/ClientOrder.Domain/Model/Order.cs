using ClientOrder.Domain.Model.Common;
using ClientOrder.Domain.Model.Enums;

namespace ClientOrder.Domain.Model;

public class Order : Entity
{
    public DateTime OrderDate { get; set; }
    public double Amount { get; set; }
    public OrderStatus Status { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
