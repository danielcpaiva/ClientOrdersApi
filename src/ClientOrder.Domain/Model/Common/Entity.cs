using ClientOrder.Domain.Interfaces;

namespace ClientOrder.Domain.Model.Common;

public abstract class Entity : IEntity
{
    public int Id { get; set; }
}
