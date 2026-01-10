using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ChakChakShop.API.DTO.Requests;
using ChakChakShop.API.DTO.Responses;
using ChakChakShop.API.Services;

namespace ChakChakShop.API.Controllers;

/// <summary>
/// Controller for managing orders
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    /// <summary>
    /// Get paginated list of orders
    /// </summary>
    /// <param name="request">Pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of orders</returns>
    /// <response code="200">Returns the paginated list of orders</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(PagedResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<OrderDto>>> GetOrders(
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.GetOrdersAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get user's orders
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user's orders</returns>
    /// <response code="200">Returns the list of user's orders</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var orders = await _orderService.GetUserOrdersAsync(userId, cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Order details</returns>
    /// <response code="200">Returns the order</response>
    /// <response code="404">Order not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        if (order == null)
        {
            return NotFound();
        }

        var userId = GetCurrentUserId();
        if (order.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Manager"))
        {
            return Forbid();
        }

        return Ok(order);
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    /// <param name="dto">Order creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created order</returns>
    /// <response code="201">Order created successfully</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderDto>> CreateOrder(
        [FromBody] CreateOrderDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var order = await _orderService.CreateOrderAsync(userId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    /// <summary>
    /// Update order status
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="status">New status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated order</returns>
    /// <response code="200">Order status updated successfully</response>
    /// <response code="404">Order not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(
        Guid id,
        [FromBody] string status,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.UpdateOrderStatusAsync(id, status, cancellationToken);
        return Ok(order);
    }

    /// <summary>
    /// Delete an order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Order deleted successfully</response>
    /// <response code="404">Order not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteOrder(Guid id, CancellationToken cancellationToken)
    {
        await _orderService.DeleteOrderAsync(id, cancellationToken);
        return NoContent();
    }
}


