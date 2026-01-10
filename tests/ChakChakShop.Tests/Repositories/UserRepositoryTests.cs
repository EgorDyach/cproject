using Microsoft.EntityFrameworkCore;
using Xunit;
using ChakChakShop.Domain.Entities;
using ChakChakShop.Infrastructure.Data;
using ChakChakShop.Infrastructure.Repositories.EfCore;

namespace ChakChakShop.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserRepository(_context);
        
        SeedData();
    }

    private void SeedData()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        // Arrange
        var userId = _context.Users.First().Id;

        // Act
        var result = await _repository.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenExists()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetByUsernameAsync_ReturnsUser_WhenExists()
    {
        // Arrange
        var username = "testuser";

        // Act
        var result = await _repository.GetByUsernameAsync(username);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
    }

    [Fact]
    public async Task AddAsync_CreatesNewUser()
    {
        // Arrange
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "newuser",
            Email = "newuser@example.com",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(newUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        var userInDb = await _context.Users.FindAsync(newUser.Id);
        Assert.NotNull(userInDb);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser()
    {
        // Arrange
        var user = _context.Users.First();
        user.Username = "updateduser";
        user.Email = "updated@example.com";

        // Act
        await _repository.UpdateAsync(user);

        // Assert
        var updatedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("updateduser", updatedUser.Username);
        Assert.Equal("updated@example.com", updatedUser.Email);
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser()
    {
        // Arrange
        var user = _context.Users.First();
        var userId = user.Id;

        // Act
        await _repository.DeleteAsync(user);

        // Assert
        var deletedUser = await _context.Users.FindAsync(userId);
        Assert.Null(deletedUser);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}


