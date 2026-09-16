using Microsoft.Extensions.Logging;
using ChakChakShop.API.DTO.Requests;
using ChakChakShop.API.DTO.Responses;
using ChakChakShop.API.Mappings;
using ChakChakShop.API.Exceptions;
using ChakChakShop.API.Repositories;

namespace ChakChakShop.API.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ICacheService cacheService,
        ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "categories_all";
        
        var cached = await _cacheService.GetAsync<IEnumerable<CategoryDto>>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var dtos = categories.Select(c => c.ToDto()).ToList();
        
        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10), cancellationToken);
        
        return dtos;
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"category_{id}";
        
        var cached = await _cacheService.GetAsync<CategoryDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return null;
        }

        var dto = category.ToDto();
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10), cancellationToken);
        
        return dto;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = dto.ToEntity();
        var created = await _categoryRepository.AddAsync(category, cancellationToken);
        
        await _cacheService.RemoveByPatternAsync("categories_*", cancellationToken);
        
        _logger.LogInformation("Category created with id {CategoryId}", created.Id);
        
        return created.ToDto();
    }

    public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException($"Category with id {id} not found");
        }

        category.UpdateEntity(dto);
        await _categoryRepository.UpdateAsync(category, cancellationToken);
        
        await _cacheService.RemoveAsync($"category_{id}", cancellationToken);
        await _cacheService.RemoveByPatternAsync("categories_*", cancellationToken);
        
        _logger.LogInformation("Category updated with id {CategoryId}", id);
        
        return category.ToDto();
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException($"Category with id {id} not found");
        }

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        
        await _cacheService.RemoveAsync($"category_{id}", cancellationToken);
        await _cacheService.RemoveByPatternAsync("categories_*", cancellationToken);
        
        _logger.LogInformation("Category deleted with id {CategoryId}", id);
    }
}


