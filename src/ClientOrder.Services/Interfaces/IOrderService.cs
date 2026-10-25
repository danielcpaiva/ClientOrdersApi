using ClientOrder.Services.Dtos;

namespace ClientOrder.Services.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderReadDto>> GetAllAsync();
    Task<OrderReadDto> GetByIdAsync(int id);
    Task<OrderReadDto> CreateAsync(OrderCreateEditDto orderDto);
    Task UpdateAsync(int id, OrderCreateEditDto orderDto);
    Task DeleteAsync(int id);
}
