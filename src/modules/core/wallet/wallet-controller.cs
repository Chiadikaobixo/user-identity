using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Wallet_service;

namespace Controllers
{
    [Route("api/wallets")]
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly WalletService _walletService;

        public WalletController(WalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetWallet()
        {
            var response = await _walletService.GetWallet<Object>();
            return new JsonResult(response);
        }
    }
}
