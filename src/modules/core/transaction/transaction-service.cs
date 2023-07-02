using System;
using Db;
using OrderEntity;
using User_Claim;
using AppResponse;
using Order_service;

namespace Transaction_service
{
    public class TransactionService
    {
        private readonly DatabaseContext _dbContext;
        private readonly Response _appResponse;
        private readonly ClaimService _claimService;
        private readonly OrderService _orderService;

        public TransactionService(DatabaseContext dbContext, Response appResponse, OrderService orderService, ClaimService claimService)
        {
            _dbContext = dbContext;
            _appResponse = appResponse;
            _claimService = claimService;
            _orderService = orderService;
        }
        public async Task<T> deposit<T>(OrderDetails orderDetails)
        {
            try
            {
                var userId = _claimService.AuthenticatedUserClaim();

                var newOrder = _orderService.logOrder(orderDetails, userId, OrderType.Deposit);

                var createOrder = await _dbContext.Orders.AddAsync(newOrder);
                if (createOrder is null)
                    return (T)_appResponse.BadRequest("Failed to create order");
                await _dbContext.SaveChangesAsync();

                var fetchOrder = await _dbContext.Orders.FindAsync(createOrder.Entity.id);
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

        public async Task<T> withdrawal<T>(OrderDetails orderDetails)
        {
            try
            {
                var userId = _claimService.AuthenticatedUserClaim();

                var newOrder = _orderService.logOrder(orderDetails, userId, OrderType.Withdrawal);

                var createOrder = await _dbContext.Orders.AddAsync(newOrder);
                if (createOrder is null)
                    return (T)_appResponse.BadRequest("Failed to create order");
                await _dbContext.SaveChangesAsync();

                var fetchOrder = await _dbContext.Orders.FindAsync(createOrder.Entity.id);
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

                var newOrder = _orderService.logOrder(orderDetails, userId, OrderType.Transfer);

                var createOrder = await _dbContext.Orders.AddAsync(newOrder);
                if (createOrder is null)
                    return (T)_appResponse.BadRequest("Failed to create order");
                await _dbContext.SaveChangesAsync();

                var fetchOrder = await _dbContext.Orders.FindAsync(createOrder.Entity.id);
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
    }
}