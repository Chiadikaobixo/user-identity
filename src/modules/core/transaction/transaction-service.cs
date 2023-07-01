using System;
using Db;
using OrderEntity;
using User_Claim;
using AppResponse;


namespace Transaction_service
{
    public class TransactionService
    {
        private readonly DatabaseContext _dbContext;
        private readonly Response _appResponse;
        private readonly ClaimService _claimService;

        public TransactionService(DatabaseContext dbContext, Response appResponse, ClaimService claimService)
        {
            _dbContext = dbContext;
            _appResponse = appResponse;
            _claimService = claimService;
        }
        public async Task<T> deposit<T>(OrderDetails orderDetails)
        {
            try
            {
                var userId = _claimService.AuthenticatedUserClaim();
                Order newOrder = new Order
                {
                    userId = userId,
                    senders_wallet_tag = orderDetails.senders_wallet_tag,
                    order_type = OrderType.Deposit,
                    amount = orderDetails.amount,
                    recievers_wallet_tag = orderDetails.recievers_wallet_tag,
                    purpose = orderDetails.purpose,
                    reference = GeneratePaymentReference()
                };

                var createOrder = await _dbContext.Orders.AddAsync(newOrder);
                if (createOrder is null)
                    return (T)_appResponse.BadRequest("Failed to create order");
                await _dbContext.SaveChangesAsync();

                return (T)_appResponse.Ok("", "Order Logged");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        public async Task<T> withdrawal<T>(OrderDetails orderDetails)
        {
            try
            {
                var userId = _claimService.AuthenticatedUserClaim();
                Order newOrder = new Order
                {
                    userId = userId,
                    senders_wallet_tag = orderDetails.senders_wallet_tag,
                    order_type = OrderType.Withdrawal,
                    amount = orderDetails.amount,
                    recievers_wallet_tag = orderDetails.recievers_wallet_tag,
                    purpose = orderDetails.purpose,
                    reference = GeneratePaymentReference()
                };

                var createOrder = await _dbContext.Orders.AddAsync(newOrder);
                if (createOrder is null)
                    return (T)_appResponse.BadRequest("Failed to create order");
                await _dbContext.SaveChangesAsync();
                return (T)_appResponse.Ok("", "Order Logged");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        public async Task<T> transfer<T>(OrderDetails orderDetails)
        {
            try
            {
                var userId = _claimService.AuthenticatedUserClaim();
                Order newOrder = new Order
                {
                    userId = userId,
                    senders_wallet_tag = orderDetails.senders_wallet_tag,
                    order_type = OrderType.Transfer,
                    amount = orderDetails.amount,
                    recievers_wallet_tag = orderDetails.recievers_wallet_tag,
                    purpose = orderDetails.purpose,
                    reference = GeneratePaymentReference()
                };

                var createOrder = await _dbContext.Orders.AddAsync(newOrder);
                if (createOrder is null)
                    return (T)_appResponse.BadRequest("Failed to create order");
                await _dbContext.SaveChangesAsync();
                return (T)_appResponse.Ok("", "Order Logged");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        string GeneratePaymentReference()
        {
            DateTime now = DateTime.Now;
            string timestamp = now.ToString("yyyyMMddHHmmss");
            string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);

            return $"{timestamp}-{uniqueId}";
        }
    }
}