using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UserEntity
{
    public class User
    {
        public Guid Id { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        public string? password { get; set; }
        public string? wallet_tag { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? date_of_birth { get; set; }
        public UserRole role { get; set; } = UserRole.User;
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public static implicit operator EntityEntry<object>(User? v)
        {
            throw new NotImplementedException();
        }
    }
}