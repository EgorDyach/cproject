using Microsoft.EntityFrameworkCore;
using ChakChakShop.API.Data.Models;
using ChakChakShop.API.Data.Context;

namespace ChakChakShop.API.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task<Order> CreateOrderWithItemsAsync(Order order, IEnumerable<OrderItem> items, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(order, cancellationToken);
        await _context.Set<OrderItem>().AddRangeAsync(items, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return order;
    }
}


