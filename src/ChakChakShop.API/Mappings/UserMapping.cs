using ChakChakShop.API.Data.Models;
using ChakChakShop.API.DTO.Responses;

namespace ChakChakShop.API.Mappings;

public static class UserMapping
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}
