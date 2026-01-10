using ChakChakShop.API.DTO.Requests;
using ChakChakShop.API.DTO.Responses;

namespace ChakChakShop.API.Services;

public interface IOrderService
{
    Task<PagedResponse<OrderDto>> GetOrdersAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<OrderDto?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto dto, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateOrderStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);
    Task DeleteOrderAsync(Guid id, CancellationToken cancellationToken = default);
}

