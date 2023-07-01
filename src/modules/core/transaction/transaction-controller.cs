using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Transaction_service;

namespace Transaction_Controllers
{
    [Route("api/transactions")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost()]
        public async Task<IActionResult> transaction(OrderType orderType, OrderDetails orderDetails)
        {
            switch (orderType)
            {
                case OrderType.Deposit:
                    var depositResponse = await _transactionService.deposit<object>(orderDetails);
                    return new JsonResult(depositResponse);

                case OrderType.Withdrawal:
                    var withdrawalResponse = await _transactionService.withdrawal<object>(orderDetails);
                    return new JsonResult(withdrawalResponse);

                case OrderType.Transfer:
                    var transferResponse = await _transactionService.transfer<object>(orderDetails);
                    return new JsonResult(transferResponse);
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderType), orderType, "Invalid order type.");
            }
        }
    }
}
