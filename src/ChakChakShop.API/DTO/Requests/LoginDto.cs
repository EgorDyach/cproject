using System.ComponentModel.DataAnnotations;

namespace ChakChakShop.API.DTO.Requests;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}


