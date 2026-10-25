using ClientOrder.Data;
using ClientOrder.Domain.Model;
using ClientOrder.Domain.Model.Enums;
using ClientOrder.Services.Dtos;
using ClientOrder.Services.Exceptions;
using ClientOrder.Services.Interfaces;
using ClientOrder.Services.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ClientOrder.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly ApplicationContext _context;
    private readonly IUserResolverService _userResolver;

    public OrderService(ApplicationContext context, IUserResolverService userResolver)
    {
        _context = context;
        _userResolver = userResolver;
    }

    public async Task<IReadOnlyList<OrderReadDto>> GetAllAsync()
    {
        var query = _context.Orders.AsNoTracking();

        if (!_userResolver.IsAdmin)
        {
            var currentUserId = _userResolver.GetById();
            query = query.Where(order => order.UserId == currentUserId);
        }

        var orders = await query
            .OrderBy(order => order.Id)
            .ToListAsync();

        return orders.Select(DtoMapper.ToDto).ToList();
    }

    public async Task<OrderReadDto> GetByIdAsync(int id)
    {
        ValidatePositiveId(id, "Order ID must be greater than zero.");

        var order = await _context.Orders
            .AsNoTracking()
            .SingleOrDefaultAsync(existingOrder => existingOrder.Id == id);

        if (order is null)
        {
            throw new ServiceNotFoundException("Order was not found.");
        }

        EnsureOwnOrder(order.UserId);

        return DtoMapper.ToDto(order);
    }

    public async Task<OrderReadDto> CreateAsync(OrderCreateEditDto orderDto)
    {
        Validate(orderDto);
        EnsureOwnOrder(orderDto.UserId);
        await EnsureUserExistsAsync(orderDto.UserId);

        var order = new Order
        {
            OrderDate = DateTime.Now,
            Amount = orderDto.Amount,
            Status = orderDto.Status,
            UserId = orderDto.UserId
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return DtoMapper.ToDto(order);
    }

    public async Task UpdateAsync(int id, OrderCreateEditDto orderDto)
    {
        ValidatePositiveId(id, "Order ID must be greater than zero.");
        Validate(orderDto);
        await EnsureUserExistsAsync(orderDto.UserId);

        var order = await _context.Orders
            .SingleOrDefaultAsync(existingOrder => existingOrder.Id == id);

        if (order is null)
        {
            throw new ServiceNotFoundException("Order was not found.");
        }

        EnsureOwnOrder(order.UserId);
        EnsureOwnOrder(orderDto.UserId);

        order.Amount = orderDto.Amount;
        order.Status = orderDto.Status;
        order.UserId = orderDto.UserId;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        ValidatePositiveId(id, "Order ID must be greater than zero.");

        var order = await _context.Orders
            .SingleOrDefaultAsync(existingOrder => existingOrder.Id == id);

        if (order is null)
        {
            throw new ServiceNotFoundException("Order was not found.");
        }

        EnsureOwnOrder(order.UserId);

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }

    private async Task EnsureUserExistsAsync(int userId)
    {
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId);

        if (!userExists)
        {
            throw new ServiceNotFoundException("User was not found.");
        }
    }

    private static void Validate(OrderCreateEditDto orderDto)
    {
        if (orderDto is null)
        {
            throw new ServiceException("Order data is required.");
        }

        if (orderDto.Amount < 0)
        {
            throw new ServiceException("Amount must be zero or greater.");
        }

        if (!Enum.IsDefined(orderDto.Status))
        {
            throw new ServiceException("Status is invalid.");
        }

        ValidatePositiveId(orderDto.UserId, "User ID must be greater than zero.");
    }

    private static void ValidatePositiveId(int id, string message)
    {
        if (id <= 0)
        {
            throw new ServiceException(message);
        }
    }

    private void EnsureOwnOrder(int ownerUserId)
    {
        if (_userResolver.IsAdmin)
        {
            return;
        }

        if (_userResolver.GetById() != ownerUserId)
        {
            throw new ServiceForbiddenException("You can only access your own orders.");
        }
    }
}
