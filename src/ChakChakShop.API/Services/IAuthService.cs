using ChakChakShop.API.DTO.Requests;
using ChakChakShop.API.DTO.Responses;

namespace ChakChakShop.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<UserDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

