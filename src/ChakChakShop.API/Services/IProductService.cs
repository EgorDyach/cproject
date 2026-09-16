using ChakChakShop.API.DTO.Requests;
using ChakChakShop.API.DTO.Responses;

namespace ChakChakShop.API.Services;

public interface IProductService
{
    Task<PagedResponse<ProductDto>> GetProductsAsync(ProductFilterDto filter, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
}

