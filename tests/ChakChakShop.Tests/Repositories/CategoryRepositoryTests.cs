using Microsoft.EntityFrameworkCore;
using Xunit;
using ChakChakShop.Domain.Entities;
using ChakChakShop.Infrastructure.Data;
using ChakChakShop.Infrastructure.Repositories.EfCore;

namespace ChakChakShop.Tests.Repositories;

public class CategoryRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CategoryRepository _repository;

    public CategoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new CategoryRepository(_context);
        
        SeedData();
    }

    private void SeedData()
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCategory_WhenExists()
    {
        // Arrange
        var categoryId = _context.Categories.First().Id;

        // Act
        var result = await _repository.GetByIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task AddAsync_CreatesNewCategory()
    {
        // Arrange
        var newCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "New Category",
            Description = "New Description",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(newCategory);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Category", result.Name);
        var categoryInDb = await _context.Categories.FindAsync(newCategory.Id);
        Assert.NotNull(categoryInDb);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesCategory()
    {
        // Arrange
        var category = _context.Categories.First();
        category.Name = "Updated Category";
        category.Description = "Updated Description";

        // Act
        await _repository.UpdateAsync(category);

        // Assert
        var updatedCategory = await _context.Categories.FindAsync(category.Id);
        Assert.NotNull(updatedCategory);
        Assert.Equal("Updated Category", updatedCategory.Name);
        Assert.Equal("Updated Description", updatedCategory.Description);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCategory()
    {
        // Arrange
        var category = _context.Categories.First();
        var categoryId = category.Id;

        // Act
        await _repository.DeleteAsync(category);

        // Assert
        var deletedCategory = await _context.Categories.FindAsync(categoryId);
        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenCategoryExists()
    {
        // Arrange
        var categoryId = _context.Categories.First().Id;

        // Act
        var result = await _repository.ExistsAsync(categoryId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenCategoryNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.ExistsAsync(nonExistentId);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}


