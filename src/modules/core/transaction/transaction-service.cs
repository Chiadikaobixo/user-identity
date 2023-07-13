using Db;
using User_Claim;
using AppResponse;
using Order_service;
using paystack_charge;
using System.Text.Json;
using Transaction_helper;

namespace Transaction_service
{
    public class TransactionService
    {
        private readonly DatabaseContext _dbContext;
        private readonly Response _appResponse;
        private readonly ClaimService _claimService;
        private readonly OrderService _orderService;
        private readonly PaystackCharge _paystackCharge;
        private readonly TransactionHelper _transactionHelper;

        public TransactionService(DatabaseContext dbContext, Response appResponse, PaystackCharge paystackCharge, OrderService orderService, TransactionHelper transactionHelper, ClaimService claimService)
        {
            _dbContext = dbContext;
            _appResponse = appResponse;
            _claimService = claimService;
            _orderService = orderService;
            _paystackCharge = paystackCharge;
            _transactionHelper = transactionHelper;
        }

        public async Task<T> deposit<T>(OrderDetails orderDetails)
        {
            try
            {
                var userId = _claimService.AuthenticatedUserClaim();
                var userEmail = _claimService.AuthenticatedEmailClaim();

                var initializeResponse = await _paystackCharge.initializeDeposit(userEmail!, orderDetails.amount);
                var responseObject = JsonSerializer.Deserialize<JsonElement>(initializeResponse);
                string reference = responseObject.GetProperty("data").GetProperty("reference").GetString() ?? string.Empty;

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
                OrderStatus order_status = OrderStatus.Successfull;
                
                var verifyResponse = await _paystackCharge.verifyTranasction(reference);
                var responseObject = JsonSerializer.Deserialize<JsonElement>(verifyResponse);
                string paymentStatus = responseObject.GetProperty("data").GetProperty("status").GetString() ?? string.Empty;
                if (paymentStatus != "success")
                    return (T)_appResponse.BadRequest("Transaction Failed");
                
                var getOrder = await _orderService.getOrder(reference);
                if(getOrder?.order_status == order_status){
                    return (T)_appResponse.BadRequest("This Order has been verified");     
                }

                int amount = responseObject.GetProperty("data").GetProperty("amount").GetInt32();
                await _transactionHelper.creditAccount<T>(amount);

                var updateOrder = await _orderService.updateOrder(reference, order_status);
                if(updateOrder is null){
                    return (T)_appResponse.BadRequest("Order Update Failed");     
                }

                return (T)_appResponse.Ok(responseObject, "Verification Status");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }
    }
}