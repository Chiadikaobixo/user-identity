using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace OrderEntity

{
    public class Order
    {
        public Guid id { get; set; }
        public Guid? userId { get; set; }
        public string? senders_wallet_tag { get; set; }
        public OrderType order_type { get; set; }
        public int amount { get; set; }
        public required string recievers_wallet_tag { get; set; }
        public string? purpose { get; set; }
        public string? reference { get; set; }
        public OrderStatus order_status {get; set;} = OrderStatus.logged;
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public static implicit operator EntityEntry<object>(Order? v)
        {
            throw new NotImplementedException();
        }

    }
}