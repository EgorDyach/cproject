using ChakChakShop.API.Data.Models;

namespace ChakChakShop.API.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Order> CreateOrderWithItemsAsync(Order order, IEnumerable<OrderItem> items, CancellationToken cancellationToken = default);
}

