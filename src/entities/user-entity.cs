using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UserEntity
{
    public class User
    {
        public Guid Id { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        public string? password { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? date_of_birth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public static implicit operator EntityEntry<object>(User? v)
        {
            throw new NotImplementedException();
        }
    }
}