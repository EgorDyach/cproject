using System.ComponentModel.DataAnnotations;

namespace ChakChakShop.API.DTO.Requests;

public class CreateCategoryDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}


