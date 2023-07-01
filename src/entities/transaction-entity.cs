using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TransactionEntity

{
    public class Transaction
    {
        public Guid id { get; set; }
        public Guid? userId { get; set; }
        public Guid? orderId { get; set; }
        public TransactionStatus transaction_status { get; set; } = TransactionStatus.Successfull;
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public static implicit operator EntityEntry<object>(Transaction? v)
        {
            throw new NotImplementedException();
        }

    }
}