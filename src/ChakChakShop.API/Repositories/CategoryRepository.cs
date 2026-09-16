using ChakChakShop.API.Data.Models;
using ChakChakShop.API.Data.Context;

namespace ChakChakShop.API.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
}


