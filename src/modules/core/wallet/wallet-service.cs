using Db;
using WalletEntity;
using User_Claim;
using AppResponse;
using Microsoft.EntityFrameworkCore;

namespace Wallet_service
{
    public class WalletService
    {
        private readonly DatabaseContext _dbContext;
        private readonly ClaimService _claimService;
        private readonly Response _appResponse;

        public WalletService(DatabaseContext dbContext, ClaimService claimService, Response appResponse)
        {
            _dbContext = dbContext;
            _claimService = claimService;
            _appResponse = appResponse;
        }

        public async Task<Wallet?> CreateWallet(Guid userId, string wallet_tag)
        {
            try
            {
                var returnedWallet = await _dbContext.Wallets.FirstOrDefaultAsync(u => u.wallet_tag == wallet_tag);
                if (returnedWallet != null)
                {
                    var user = await _dbContext.Users.FindAsync(userId);
                    if (user != null)
                    {
                        _dbContext.Users.Remove(user);
                        await _dbContext.SaveChangesAsync();
                    }
                    return null;
                }

                Wallet createWallet = new Wallet
                {
                    userId = userId,
                    wallet_tag = wallet_tag
                };

                await _dbContext.Wallets.AddAsync(createWallet);
                await _dbContext.SaveChangesAsync();

                return createWallet;
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        public async Task<T> GetWallet<T>()
        {
            try
            {
                var userId =  _claimService.AuthenticatedUserClaim();

                var returnedWallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.userId == userId);
                if (returnedWallet is null)
                    return (T)_appResponse.BadRequest("Wallet Not Found");
                return (T)_appResponse.Ok(returnedWallet, "Wallet Details");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }
    }
}
