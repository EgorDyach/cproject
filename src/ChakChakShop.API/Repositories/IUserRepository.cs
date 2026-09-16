using ChakChakShop.API.Data.Models;

namespace ChakChakShop.API.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetUserWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}

