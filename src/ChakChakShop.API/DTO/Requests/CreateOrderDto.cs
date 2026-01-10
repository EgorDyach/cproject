using System.ComponentModel.DataAnnotations;

namespace ChakChakShop.API.DTO.Requests;

public class CreateOrderDto
{
    [Required]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}


