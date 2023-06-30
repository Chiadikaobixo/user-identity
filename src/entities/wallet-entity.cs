using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WalletEntity

{
    public class Wallet
    {
        public Guid id { get; set; }
        public Guid userId { get; set; }
        public string? wallet_tag {get; set;}
        public int? account_balance { get; set; } = (int)0.00;
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public static implicit operator EntityEntry<object>(Wallet? v)
        {
            throw new NotImplementedException();
        }

    }
}