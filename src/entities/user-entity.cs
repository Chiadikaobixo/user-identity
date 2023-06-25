using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UserEntity
{
    public class User
    {
        public Guid Id { get; set; }
        public required string email { get; set; }
        public required string password { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? date_of_birth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public static implicit operator EntityEntry<object>(User? v)
        {
            throw new NotImplementedException();
        }
    }
}