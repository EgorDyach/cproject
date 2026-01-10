using ChakChakShop.API.Data.Models;

namespace ChakChakShop.API.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsWithIngredientsAsync(CancellationToken cancellationToken = default);
}

