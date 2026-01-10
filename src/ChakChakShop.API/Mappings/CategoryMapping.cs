using ChakChakShop.API.Data.Models;
using ChakChakShop.API.DTO.Requests;
using ChakChakShop.API.DTO.Responses;

namespace ChakChakShop.API.Mappings;

public static class CategoryMapping
{
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public static Category ToEntity(this CreateCategoryDto dto)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this Category category, UpdateCategoryDto dto)
    {
        category.Name = dto.Name;
        category.Description = dto.Description;
        category.UpdatedAt = DateTime.UtcNow;
    }
}
