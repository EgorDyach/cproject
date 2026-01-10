using Microsoft.Extensions.Logging;
using ChakChakShop.API.DTO.Requests;
using ChakChakShop.API.DTO.Responses;
using ChakChakShop.API.Data.Models;
using ChakChakShop.API.Mappings;
using ChakChakShop.API.Exceptions;
using ChakChakShop.API.Repositories;

namespace ChakChakShop.API.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResponse<OrderDto>> GetOrdersAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        var totalCount = orders.Count();
        var pagedOrders = orders.Skip(request.Skip).Take(request.PageSize).ToList();

        return new PagedResponse<OrderDto>
        {
            Data = pagedOrders.Select(o => o.ToDto()),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(id, cancellationToken);
        return order?.ToDto();
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, cancellationToken);
        return orders.Select(o => o.ToDto());
    }

    public async Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var itemDto in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException($"Product with id {itemDto.ProductId} not found");
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                throw new BadRequestException($"Insufficient stock for product {product.Name}. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}");
            }

            var unitPrice = product.Price;
            var totalPrice = unitPrice * itemDto.Quantity;
            totalAmount += totalPrice;

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = totalPrice
            };

            orderItems.Add(orderItem);

            product.StockQuantity -= itemDto.Quantity;
            await _productRepository.UpdateAsync(product, cancellationToken);
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TotalAmount = totalAmount,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        var createdOrder = await _orderRepository.CreateOrderWithItemsAsync(order, orderItems, cancellationToken);
        
        _logger.LogInformation("Order created with id {OrderId} for user {UserId}", createdOrder.Id, userId);
        
        var orderWithItems = await _orderRepository.GetOrderWithItemsAsync(createdOrder.Id, cancellationToken);
        return orderWithItems!.ToDto();
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(Guid id, string status, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException($"Order with id {id} not found");
        }

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order, cancellationToken);
        
        _logger.LogInformation("Order status updated to {Status} for order {OrderId}", status, id);
        
        var updatedOrder = await _orderRepository.GetOrderWithItemsAsync(id, cancellationToken);
        return updatedOrder!.ToDto();
    }

    public async Task DeleteOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException($"Order with id {id} not found");
        }

        await _orderRepository.DeleteAsync(order, cancellationToken);
        
        _logger.LogInformation("Order deleted with id {OrderId}", id);
    }
}


