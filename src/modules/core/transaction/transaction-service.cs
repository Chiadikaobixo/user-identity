using Db;
using User_Claim;
using AppResponse;
using Order_service;
using paystack_charge;
using System.Text.Json;

namespace Transaction_service
{
    public class TransactionService
    {
        private readonly DatabaseContext _dbContext;
        private readonly Response _appResponse;
        private readonly ClaimService _claimService;
        private readonly OrderService _orderService;
        private readonly PaystackCharge _paystackCharge;

        public TransactionService(DatabaseContext dbContext, Response appResponse, PaystackCharge paystackCharge, OrderService orderService, ClaimService claimService)
        {
            _dbContext = dbContext;
            _appResponse = appResponse;
            _claimService = claimService;
            _orderService = orderService;
            _paystackCharge = paystackCharge;
        }

        public async Task<T> deposit<T>(OrderDetails orderDetails)
        {
            try
            {
                var userId = _claimService.AuthenticatedUserClaim();
                var userEmail = _claimService.AuthenticatedEmailClaim();

                var initializeResponse = await _paystackCharge.initializeDeposit(userEmail!, orderDetails.amount);
                var responseObject = JsonSerializer.Deserialize<JsonElement>(initializeResponse);
                string reference = responseObject.GetProperty("data").GetProperty("reference").GetString();

                var createOrder = await _orderService.logOrder(orderDetails, userId, OrderType.Deposit, reference);
                if (createOrder is null)
                    return (T)_appResponse.BadRequest("Failed to create order");

                return (T)_appResponse.Ok(responseObject, "Order Logged");
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

                var createOrder = await _orderService.logOrder(orderDetails, userId, OrderType.Withdrawal, null);
                if (createOrder is null)
                    return (T)_appResponse.BadRequest("Failed to create order");

                var fetchOrder = await _dbContext.Orders.FindAsync(createOrder.id);
                if (fetchOrder is null)
                    return (T)_appResponse.BadRequest("Could not create and order");

                return (T)_appResponse.Ok(fetchOrder, "Order Logged");
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

                var createOrder = await _orderService.logOrder(orderDetails, userId, OrderType.Transfer, null);
                if (createOrder is null)
                    return (T)_appResponse.BadRequest("Failed to create order");

                var fetchOrder = await _dbContext.Orders.FindAsync(createOrder.id);
                if (fetchOrder is null)
                    return (T)_appResponse.BadRequest("Could not create and order");

                return (T)_appResponse.Ok(fetchOrder, "Order Logged");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        public async Task<T> verifyTransaction<T>(string reference)
        {
            try
            {
                var verifyResponse = await _paystackCharge.verifyTranasction(reference);
                var responseObject = JsonSerializer.Deserialize<JsonElement>(verifyResponse);
                string paymentStatus = responseObject.GetProperty("data").GetProperty("status").GetString();
                if (paymentStatus != "success")
                    return (T)_appResponse.BadRequest("Transaction Failed");
                
                // Check if a reference or order has been updated here

                // Run the credit or debit logic here before you update the orderstatus

                OrderStatus order_status = OrderStatus.Successfull;
                var updateOrder = await _orderService.updateOrder(reference, order_status);
                if(updateOrder is null){
                    return (T)_appResponse.BadRequest("Order Update Failed");     
                }

                return (T)_appResponse.Ok(responseObject, "Verification Status");
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}