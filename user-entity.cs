using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UserEntity
{
    public class User
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public int Age { get; set; }

        public static implicit operator EntityEntry<object>(User? v)
        {
            throw new NotImplementedException();
        }
    }
}