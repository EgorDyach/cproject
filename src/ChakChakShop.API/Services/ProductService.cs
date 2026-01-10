using Microsoft.Extensions.Logging;
using ChakChakShop.API.DTO.Requests;
using ChakChakShop.API.DTO.Responses;
using ChakChakShop.API.Mappings;
using ChakChakShop.API.Exceptions;
using ChakChakShop.API.Repositories;

namespace ChakChakShop.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICacheService cacheService,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<PagedResponse<ProductDto>> GetProductsAsync(ProductFilterDto filter, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"products_{filter.PageNumber}_{filter.PageSize}_{filter.Name}_{filter.CategoryId}_{filter.MinPrice}_{filter.MaxPrice}_{filter.InStock}";
        
        var cached = await _cacheService.GetAsync<PagedResponse<ProductDto>>(cacheKey, cancellationToken);
        if (cached != null)
        {
            _logger.LogInformation("Products retrieved from cache");
            return cached;
        }

        var products = await _productRepository.GetProductsWithIngredientsAsync(cancellationToken);
        
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            products = products.Where(p => p.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
        }
        
        if (filter.CategoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == filter.CategoryId.Value);
        }
        
        if (filter.MinPrice.HasValue)
        {
            products = products.Where(p => p.Price >= filter.MinPrice.Value);
        }
        
        if (filter.MaxPrice.HasValue)
        {
            products = products.Where(p => p.Price <= filter.MaxPrice.Value);
        }
        
        if (filter.InStock.HasValue && filter.InStock.Value)
        {
            products = products.Where(p => p.StockQuantity > 0);
        }

        var totalCount = products.Count();
        var pagedProducts = products.Skip(filter.Skip).Take(filter.PageSize).ToList();

        var response = new PagedResponse<ProductDto>
        {
            Data = pagedProducts.Select(p => p.ToDto()),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);
        
        return response;
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"product_{id}";
        
        var cached = await _cacheService.GetAsync<ProductDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return null;
        }

        var dto = product.ToDto();
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5), cancellationToken);
        
        return dto;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var categoryExists = await _categoryRepository.ExistsAsync(dto.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new NotFoundException($"Category with id {dto.CategoryId} not found");
        }

        var product = dto.ToEntity();
        var created = await _productRepository.AddAsync(product, cancellationToken);
        
        await _cacheService.RemoveByPatternAsync("products_*", cancellationToken);
        await _cacheService.RemoveByPatternAsync("categories_*", cancellationToken);
        
        _logger.LogInformation("Product created with id {ProductId}", created.Id);
        
        return created.ToDto();
    }

    public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            throw new NotFoundException($"Product with id {id} not found");
        }

        var categoryExists = await _categoryRepository.ExistsAsync(dto.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new NotFoundException($"Category with id {dto.CategoryId} not found");
        }

        product.UpdateEntity(dto);
        await _productRepository.UpdateAsync(product, cancellationToken);
        
        await _cacheService.RemoveAsync($"product_{id}", cancellationToken);
        await _cacheService.RemoveByPatternAsync("products_*", cancellationToken);
        
        _logger.LogInformation("Product updated with id {ProductId}", id);
        
        return product.ToDto();
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            throw new NotFoundException($"Product with id {id} not found");
        }

        await _productRepository.DeleteAsync(product, cancellationToken);
        
        await _cacheService.RemoveAsync($"product_{id}", cancellationToken);
        await _cacheService.RemoveByPatternAsync("products_*", cancellationToken);
        
        _logger.LogInformation("Product deleted with id {ProductId}", id);
    }
}


