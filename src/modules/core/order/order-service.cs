using OrderEntity;

namespace Order_service
{
    public class OrderService
    {
        public Order logOrder(OrderDetails orderDetails, Guid? userId, OrderType ordertype)
        {
            try
            {
                Order newOrder = new Order
                {
                    userId = userId,
                    senders_wallet_tag = orderDetails.senders_wallet_tag,
                    order_type = ordertype,
                    amount = orderDetails.amount,
                    recievers_wallet_tag = orderDetails.recievers_wallet_tag,
                    purpose = orderDetails.purpose,
                    reference = GeneratePaymentReference()
                };
                return newOrder;
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }
        
        public string GeneratePaymentReference()
        {
            try
            {
                DateTime now = DateTime.Now;
                string timestamp = now.ToString("yyyyMMddHHmmss");
                string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);

                return $"{timestamp}-{uniqueId}";
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }
    }
}