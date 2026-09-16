namespace ChakChakShop.API.Data.Models;

public class Ingredient
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
}


