using AppResponse;
using Db;
using UserEntity;

namespace Auth_Services
{
    public class AuthServices
    {
        private readonly DatabaseContext _dbContext;

        private readonly Response _appResponse;

        public AuthServices(DatabaseContext dbContext, Response appResponse)
        {
            _dbContext = dbContext;
            _appResponse = appResponse;

        }
        public async Task<T> CreateUser<T>(User user)
        {
            try
            {
                var createUser = await _dbContext.Users.AddAsync(user);
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
    }
}