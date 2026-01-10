using Microsoft.AspNetCore.Mvc;
using ChakChakShop.API.DTO.Requests;
using ChakChakShop.API.DTO.Responses;
using ChakChakShop.API.Services;

namespace ChakChakShop.API.Controllers;

/// <summary>
/// Controller for authentication
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="dto">Registration data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with JWT token</returns>
    /// <response code="201">User registered successfully</response>
    /// <response code="400">Invalid input data or user already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetCurrentUser), new { }, result);
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with JWT token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="400">Invalid credentials</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current user details</returns>
    /// <response code="200">Returns the current user</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
        var user = await _authService.GetCurrentUserAsync(userId, cancellationToken);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}


