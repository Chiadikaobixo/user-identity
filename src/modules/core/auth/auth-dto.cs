using System.ComponentModel.DataAnnotations;

namespace AuthDTO
{
    public record AuthUserDTO(
        [Required]
        [EmailAddress]
        string email,
        string password
    );
}