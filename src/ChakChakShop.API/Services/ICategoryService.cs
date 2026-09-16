using ChakChakShop.API.DTO.Requests;
using ChakChakShop.API.DTO.Responses;

namespace ChakChakShop.API.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
}

