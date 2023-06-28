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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.email)
                .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
         public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

         private void UpdateTimestamps()
        {
            var entities = ChangeTracker.Entries<User>();
            var currentTime = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Entity.CreatedAt = currentTime;
                    entity.Entity.UpdatedAt = currentTime;
                }
                else if (entity.State == EntityState.Modified)
                {
                    entity.Entity.UpdatedAt = currentTime;
                }
            }
        }
    }
}
