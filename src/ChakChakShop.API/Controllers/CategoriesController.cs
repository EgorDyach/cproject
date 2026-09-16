using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChakChakShop.API.DTO.Requests;
using ChakChakShop.API.DTO.Responses;
using ChakChakShop.API.Services;

namespace ChakChakShop.API.Controllers;

/// <summary>
/// Controller for managing categories
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of categories</returns>
    /// <response code="200">Returns the list of categories</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        return Ok(categories);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category details</returns>
    /// <response code="200">Returns the category</response>
    /// <response code="404">Category not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategory(Guid id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="dto">Category creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category</returns>
    /// <response code="201">Category created successfully</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CategoryDto>> CreateCategory(
        [FromBody] CreateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var category = await _categoryService.CreateCategoryAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="dto">Category update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category</returns>
    /// <response code="200">Category updated successfully</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="404">Category not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var category = await _categoryService.UpdateCategoryAsync(id, dto, cancellationToken);
        return Ok(category);
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Category deleted successfully</response>
    /// <response code="404">Category not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteCategoryAsync(id, cancellationToken);
        return NoContent();
    }
}


