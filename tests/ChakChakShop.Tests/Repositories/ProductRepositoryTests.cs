using Microsoft.EntityFrameworkCore;
using Xunit;
using ChakChakShop.Domain.Entities;
using ChakChakShop.Infrastructure.Data;
using ChakChakShop.Infrastructure.Repositories.EfCore;

namespace ChakChakShop.Tests.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ProductRepository(_context);
        
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

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Description = "Test Description",
            Price = 100.50m,
            StockQuantity = 10,
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow
        };
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsProduct_WhenExists()
    {
        // Arrange
        var productId = _context.Products.First().Id;

        // Act
        var result = await _repository.GetByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
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
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task AddAsync_CreatesNewProduct()
    {
        // Arrange
        var category = _context.Categories.First();
        var newProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "New Product",
            Description = "New Description",
            Price = 200.00m,
            StockQuantity = 5,
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(newProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        var productInDb = await _context.Products.FindAsync(newProduct.Id);
        Assert.NotNull(productInDb);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesProduct()
    {
        // Arrange
        var product = _context.Products.First();
        product.Name = "Updated Product";
        product.Price = 150.00m;

        // Act
        await _repository.UpdateAsync(product);

        // Assert
        var updatedProduct = await _context.Products.FindAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Product", updatedProduct.Name);
        Assert.Equal(150.00m, updatedProduct.Price);
    }

    [Fact]
    public async Task DeleteAsync_RemovesProduct()
    {
        // Arrange
        var product = _context.Products.First();
        var productId = product.Id;

        // Act
        await _repository.DeleteAsync(product);

        // Assert
        var deletedProduct = await _context.Products.FindAsync(productId);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenProductExists()
    {
        // Arrange
        var productId = _context.Products.First().Id;

        // Act
        var result = await _repository.ExistsAsync(productId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenProductNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.ExistsAsync(nonExistentId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ReturnsProductsInCategory()
    {
        // Arrange
        var category = _context.Categories.First();

        // Act
        var result = await _repository.GetByCategoryIdAsync(category.Id);

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, p => Assert.Equal(category.Id, p.CategoryId));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}


