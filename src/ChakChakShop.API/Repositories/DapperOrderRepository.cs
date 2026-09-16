using System.Data;
using System.Linq.Expressions;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ChakChakShop.API.Data.Connections;
using ChakChakShop.API.Data.Models;

namespace ChakChakShop.API.Repositories;

public class DapperOrderRepository : IOrderRepository
{
    private readonly IDbConnectionFactory _connections;

    static DapperOrderRepository()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public DapperOrderRepository(IDbConnectionFactory connections)
    {
        _connections = connections;
    }

    /// <summary>Primary: запись и те чтения, которым нельзя видеть устаревшие данные.</summary>
    private IDbConnection CreateConnection() => _connections.CreateWriteConnection();

    /// <summary>
    /// Replica: списки и счётчики. Эти ответы переживают отставание на доли
    /// секунды — страница заказов, опоздавшая на один свежий заказ, ничего
    /// не ломает. Точечные чтения по id сюда не переводятся: сразу после
    /// POST /api/orders клиент запрашивает созданный заказ по идентификатору,
    /// и на реплике его может ещё не быть.
    /// </summary>
    private IDbConnection CreateReadConnection() => _connections.CreateReadConnection();

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT id, user_id AS UserId, total_amount AS TotalAmount, status, created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM orders
            WHERE id = @Id";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Order>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT id, user_id AS UserId, total_amount AS TotalAmount, status, created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM orders
            ORDER BY created_at DESC";

        using var connection = CreateConnection();
        return await connection.QueryAsync<Order>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Order>> FindAsync(Expression<Func<Order, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        var compiled = predicate.Compile();
        return all.Where(compiled);
    }

    public async Task<Order> AddAsync(Order entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO orders (id, user_id, total_amount, status, created_at, updated_at)
            VALUES (@Id, @UserId, @TotalAmount, @Status, @CreatedAt, @UpdatedAt)
            RETURNING id, user_id AS UserId, total_amount AS TotalAmount, status, created_at AS CreatedAt, updated_at AS UpdatedAt";

        using var connection = CreateConnection();
        return await connection.QueryFirstAsync<Order>(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE orders
            SET user_id = @UserId, total_amount = @TotalAmount, status = @Status, updated_at = @UpdatedAt
            WHERE id = @Id";

        using var connection = CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(Order entity, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM orders WHERE id = @Id";

        using var connection = CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = entity.Id }, cancellationToken: cancellationToken));
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM orders WHERE id = @Id";

        using var connection = CreateConnection();
        var count = await connection.QuerySingleAsync<int>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return count > 0;
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT id, user_id AS UserId, total_amount AS TotalAmount, status, created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM orders
            WHERE user_id = @UserId
            ORDER BY created_at DESC";

        using var connection = CreateConnection();
        return await connection.QueryAsync<Order>(
            new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Order>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        // LIMIT/OFFSET в SQL: без него пришлось бы вычитывать всю таблицу
        // и отбрасывать лишнее в памяти приложения.
        // Опирается на индекс IX_orders_created_at (миграция 004).
        const string sql = @"
            SELECT id, user_id AS UserId, total_amount AS TotalAmount, status, created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM orders
            ORDER BY created_at DESC
            LIMIT @Take OFFSET @Skip";

        using var connection = CreateReadConnection();
        return await connection.QueryAsync<Order>(
            new CommandDefinition(sql, new { Take = take, Skip = skip }, cancellationToken: cancellationToken));
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(*) FROM orders";

        using var connection = CreateReadConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Order>> GetByUserIdPagedAsync(Guid userId, int skip, int take, CancellationToken cancellationToken = default)
    {
        // Опирается на индекс IX_orders_user_id_created_at (миграция 004):
        // user_id закрывает условие равенства, created_at DESC — сортировку,
        // поэтому план обходится без узла Sort и останавливается на LIMIT.
        const string sql = @"
            SELECT id, user_id AS UserId, total_amount AS TotalAmount, status, created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM orders
            WHERE user_id = @UserId
            ORDER BY created_at DESC
            LIMIT @Take OFFSET @Skip";

        using var connection = CreateReadConnection();
        return await connection.QueryAsync<Order>(
            new CommandDefinition(sql, new { UserId = userId, Take = take, Skip = skip }, cancellationToken: cancellationToken));
    }

    public async Task<int> GetCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(*) FROM orders WHERE user_id = @UserId";

        using var connection = CreateReadConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
    }

    public async Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        const string orderSql = @"
            SELECT id, user_id AS UserId, total_amount AS TotalAmount, status, created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM orders
            WHERE id = @OrderId";

        const string itemsSql = @"
            SELECT id, order_id AS OrderId, product_id AS ProductId, quantity, unit_price AS UnitPrice, total_price AS TotalPrice
            FROM order_items
            WHERE order_id = @OrderId";

        using var connection = CreateConnection();
        var order = await connection.QueryFirstOrDefaultAsync<Order>(
            new CommandDefinition(orderSql, new { OrderId = orderId }, cancellationToken: cancellationToken));

        if (order != null)
        {
            var items = await connection.QueryAsync<OrderItem>(
                new CommandDefinition(itemsSql, new { OrderId = orderId }, cancellationToken: cancellationToken));
            if (items != null)
            {
                order.OrderItems = items.ToList();
            }
        }

        return order;
    }

    public async Task<Order> CreateOrderWithItemsAsync(Order order, IEnumerable<OrderItem> items, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string orderSql = @"
                INSERT INTO orders (id, user_id, total_amount, status, created_at, updated_at)
                VALUES (@Id, @UserId, @TotalAmount, @Status, @CreatedAt, @UpdatedAt)
                RETURNING id, user_id AS UserId, total_amount AS TotalAmount, status, created_at AS CreatedAt, updated_at AS UpdatedAt";

            var createdOrder = await connection.QueryFirstAsync<Order>(
                new CommandDefinition(orderSql, order, transaction, cancellationToken: cancellationToken));

            const string itemsSql = @"
                INSERT INTO order_items (id, order_id, product_id, quantity, unit_price, total_price)
                VALUES (@Id, @OrderId, @ProductId, @Quantity, @UnitPrice, @TotalPrice)";

            await connection.ExecuteAsync(
                new CommandDefinition(itemsSql, items, transaction, cancellationToken: cancellationToken));

            transaction.Commit();
            return createdOrder;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}

