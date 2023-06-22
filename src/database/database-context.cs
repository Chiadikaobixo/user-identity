using Microsoft.EntityFrameworkCore;
using UserEntity;

namespace Db
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) :
            base(options)
        {
        }

        public required DbSet<User> Users { get; set; }
    }
}
