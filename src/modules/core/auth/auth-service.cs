using AppResponse;
using Db;
using UserEntity;
using AuthDTO;
using Hash;
using Jwt;

namespace Auth_Services
{
    public class AuthServices
    {
        private readonly DatabaseContext _dbContext;
        private readonly Response _appResponse;
        private readonly Hashed _hashed;
        private readonly Token _token;

        public AuthServices(DatabaseContext dbContext, Response appResponse, Hashed hashed, Token token)
        {
            _dbContext = dbContext;
            _appResponse = appResponse;
            _hashed = hashed; 
            _token = token;
        }
        public async Task<T> CreateUser<T>(AuthUserDTO authuser)
        {
            try
            {                
                string hashed_password = _hashed.hashedPassword(authuser.password);
                User userEntity = new User
                {
                    email = authuser.email,
                    password = hashed_password,
                };
                
                var createUser = await _dbContext.Users.AddAsync(userEntity);
                if (createUser is null)
                    return (T)_appResponse.BadRequest("User Not Created");
                await _dbContext.SaveChangesAsync();

                var fetcheduser = await _dbContext.Users.FindAsync(createUser.Entity.Id);
                if (fetcheduser is null)
                    return (T)_appResponse.BadRequest("Could not fetch user");
                return (T)_appResponse.Ok(fetcheduser, "User Created");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        public async Task<T> LoginUser<T>(AuthUserDTO authuser)
        {
            try
            {
                var existingUser = _dbContext.Users.FirstOrDefault(u => u.email == authuser.email);
                if (existingUser is null)
                    return (T)_appResponse.BadRequest("Loggin Failed");
                
                bool isPasswordValid = _hashed.validPassword(authuser.password, existingUser.password);
                if(!isPasswordValid)
                    return (T)_appResponse.BadRequest("Loggin Failed");

                var jwtToken = _token.generate(existingUser);
                var response = new {
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