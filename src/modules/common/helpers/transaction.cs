using System.Text.Json;
using Wallet_service;

namespace Transaction_helper
{
    public class TransactionHelper
    {
        private readonly WalletService _walletService;

        public TransactionHelper(WalletService walletService)
        {
            _walletService = walletService;
        }
        public async Task creditAccount<T>(int amount)
        {
            try
            {
                var amountToCredit = amount / 100;
                await _walletService.updateWallet<T>(amountToCredit);
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        public void debitAccount()
        {
            try
            {

            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }
    }
}