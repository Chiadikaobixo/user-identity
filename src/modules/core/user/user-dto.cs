namespace UserDTO
{
    public record UserUpdateDTO(
        string? first_name,
        string? last_name,
        string? password,
        string? date_of_birth
    );
}