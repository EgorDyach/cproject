namespace ChakChakShop.API.Data.Models;

public class ProductIngredient
{
    public Guid ProductId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    
    public Product Product { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
}


