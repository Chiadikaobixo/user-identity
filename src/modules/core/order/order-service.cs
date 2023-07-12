using OrderEntity;
using Db;
using AppResponse;
using Microsoft.EntityFrameworkCore;

namespace Order_service
{
    public class OrderService
    {
        private readonly DatabaseContext _dbContext;
        private readonly Response _appResponse;

        public OrderService(DatabaseContext dbContext, Response appResponse){
            _dbContext = dbContext;
            _appResponse = appResponse;

        }
        public async Task<Order> logOrder(OrderDetails orderDetails, Guid? userId, OrderType ordertype, string? paymentReference)
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
                    reference = paymentReference
                };

                var createOrder = await _dbContext.Orders.AddAsync(newOrder);
                if (createOrder.Entity is null) return null;
                await _dbContext.SaveChangesAsync();

                return createOrder.Entity;
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }
        
        public async Task<Order> updateOrder(string reference, OrderStatus order_status){
            try
            {
                var fetchOrder = await _dbContext.Orders.FirstOrDefaultAsync(o => o.reference == reference);
                if(fetchOrder is null) return null;
                
                if (order_status != null)
                    fetchOrder.order_status = order_status;
                
                var updateOrder = _dbContext.Orders.Update(fetchOrder);
                await _dbContext.SaveChangesAsync();

                return fetchOrder;
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