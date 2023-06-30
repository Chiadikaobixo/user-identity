using System.ComponentModel.DataAnnotations;

namespace AuthDTO
{
    public record CreateUserDTO(
        [Required]
        [EmailAddress]
        string email,
        [Required]
        string wallet_tag,
        [Required]
        string password
    );

    public record LoginDTO(
        [Required]
        [EmailAddress]
        string email,
        [Required]
        string password
    );
}