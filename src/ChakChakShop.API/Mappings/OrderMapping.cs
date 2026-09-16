using ChakChakShop.API.Data.Models;
using ChakChakShop.API.DTO.Responses;

namespace ChakChakShop.API.Mappings;

public static class OrderMapping
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.OrderItems.Select(oi => oi.ToDto()).ToList()
        };
    }

    public static OrderItemDto ToDto(this OrderItem orderItem)
    {
        return new OrderItemDto
        {
            Id = orderItem.Id,
            ProductId = orderItem.ProductId,
            ProductName = orderItem.Product?.Name,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
            TotalPrice = orderItem.TotalPrice
        };
    }
}
