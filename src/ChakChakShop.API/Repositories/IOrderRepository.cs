using ChakChakShop.API.Data.Models;

namespace ChakChakShop.API.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Одна страница заказов, отсортированных по дате создания.
    /// Пагинация выполняется в SQL (LIMIT/OFFSET), а не в памяти приложения.
    /// </summary>
    Task<IEnumerable<Order>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default);

    /// <summary>Общее количество заказов — для поля TotalCount в PagedResponse.</summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>Одна страница заказов конкретного пользователя.</summary>
    Task<IEnumerable<Order>> GetByUserIdPagedAsync(Guid userId, int skip, int take, CancellationToken cancellationToken = default);

    /// <summary>Количество заказов конкретного пользователя.</summary>
    Task<int> GetCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Order> CreateOrderWithItemsAsync(Order order, IEnumerable<OrderItem> items, CancellationToken cancellationToken = default);
}

