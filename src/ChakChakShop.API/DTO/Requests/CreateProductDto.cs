using System.ComponentModel.DataAnnotations;

namespace ChakChakShop.API.DTO.Requests;

public class CreateProductDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
    public int StockQuantity { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}


