using Microsoft.EntityFrameworkCore;
using ChakChakShop.API.Data.Models;
using ChakChakShop.API.Data.Context;

namespace ChakChakShop.API.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsWithIngredientsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
            .ToListAsync(cancellationToken);
    }
}


