using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserEntity;
using WalletEntity;
using TransactionEntity;
using OrderEntity;

namespace Db
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) :
            base(options)
        {
        }

        public required DbSet<User> Users { get; set; }
        public required DbSet<Wallet> Wallets { get; set; }
        public required DbSet<Transaction> Transactions { get; set; }
        public required DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.email)
                .IsUnique();
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Wallet>()
                .HasIndex(u => u.wallet_tag)
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
            var userEntries = ChangeTracker.Entries<User>().Cast<EntityEntry>();
            var walletEntries = ChangeTracker.Entries<Wallet>().Cast<EntityEntry>();
            var orderEntries = ChangeTracker.Entries<Order>().Cast<EntityEntry>();
            var transactionEntries = ChangeTracker.Entries<Transaction>().Cast<EntityEntry>();

            var entities = userEntries.Concat<EntityEntry>(walletEntries).Concat<EntityEntry>(orderEntries).Concat<EntityEntry>(transactionEntries);

            var currentTime = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    if (entity.Entity is User user)
                    {
                        user.created_at = currentTime;
                        user.updated_at = currentTime;
                    }
                    else if (entity.Entity is Wallet wallet)
                    {
                        wallet.created_at = currentTime;
                        wallet.updated_at = currentTime;
                    }
                    else if (entity.Entity is Order order)
                    {
                        order.created_at = currentTime;
                        order.updated_at = currentTime;
                    }
                    else if (entity.Entity is Transaction transaction)
                    {
                        transaction.created_at = currentTime;
                        transaction.updated_at = currentTime;
                    }
                }
                else if (entity.State == EntityState.Modified)
                {
                    if (entity.Entity is User user)
                    {
                        user.updated_at = currentTime;
                    }
                    else if (entity.Entity is Wallet wallet)
                    {
                        wallet.updated_at = currentTime;
                    }
                    else if (entity.Entity is Order order)
                    {
                        order.updated_at = currentTime;
                    }
                    else if (entity.Entity is Transaction transaction)
                    {
                        transaction.updated_at = currentTime;
                    }
                }
            }
        }
    }
}
