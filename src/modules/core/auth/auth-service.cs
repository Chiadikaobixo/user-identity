using AppResponse;
using Db;
using UserEntity;
using AuthDTO;
using Hash;
using Jwt;
using Microsoft.EntityFrameworkCore;
using Wallet_service;

namespace Auth_Services
{
    public class AuthServices
    {
        private readonly DatabaseContext _dbContext;
        private readonly Response _appResponse;
        private readonly WalletService _walletService;
        private readonly Hashed _hashed;
        private readonly Token _token;

        public AuthServices(DatabaseContext dbContext, Response appResponse, Hashed hashed, Token token, WalletService walletService)
        {
            _dbContext = dbContext;
            _appResponse = appResponse;
            _walletService = walletService;
            _hashed = hashed;
            _token = token;
        }
        public async Task<T> CreateUser<T>(CreateUserDTO authuser)
        {
            try
            {                
                var userExist = await _dbContext.Users.FirstOrDefaultAsync(u => u.email == authuser.email);
                if (userExist != null)
                    return (T)_appResponse.BadRequest("User Already Exist");

                string hashed_password = _hashed.hashedPassword(authuser.password);
                User userEntity = new User
                {
                    wallet_tag = authuser.wallet_tag,
                    email = authuser.email,
                    password = hashed_password,
                };

                var createUser = await _dbContext.Users.AddAsync(userEntity);
                if (createUser is null)
                    return (T)_appResponse.BadRequest("User Not Created");
                await _dbContext.SaveChangesAsync();

                var walletCreated = await _walletService.CreateWallet(createUser.Entity.Id, createUser.Entity.wallet_tag!);
                if(walletCreated is null)
                    return (T)_appResponse.BadRequest("Wallet UserName Already Exist");
                
                var fetcheduser = await _dbContext.Users.FindAsync(createUser.Entity.Id);
                if (fetcheduser is null)
                    return (T)_appResponse.BadRequest("Could not create user");
                
                return (T)_appResponse.Ok(fetcheduser, "User Created");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        public async Task<T> LoginUser<T>(LoginDTO authuser)
        {
            try
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.email == authuser.email);
                if (existingUser is null)
                    return (T)_appResponse.BadRequest("User does not exist");

                bool isPasswordValid = _hashed.validPassword(authuser.password, existingUser.password!);
                if (!isPasswordValid)
                    return (T)_appResponse.BadRequest("Loggin Failed");

                var jwtToken = _token.generate(existingUser);
                var response = new
                {
                    user = existingUser,
                    token = jwtToken
                };

                return (T)_appResponse.Ok(response, "Logged In Successfull");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }
    }
}