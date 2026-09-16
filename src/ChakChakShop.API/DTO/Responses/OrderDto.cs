namespace ChakChakShop.API.DTO.Responses;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

