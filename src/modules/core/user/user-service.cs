using AppResponse;
using Db;
using UserEntity;

namespace Services
{
    public class UserService
    {
        private readonly DatabaseContext _dbContext;

        private readonly Response _appResponse;

        public UserService(DatabaseContext dbContext, Response appResponse)
        {
            _dbContext = dbContext;
            _appResponse = appResponse;
        }

        public async Task<T> GetUser<T>(Guid id)
        {
            try
            {
                var returnedUser = await _dbContext.Users.FindAsync(id);
                if (returnedUser is null)
                    return (T)_appResponse.BadRequest("User Not Found");
                return (T)_appResponse.Ok(returnedUser, "User Details");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        public async Task<T> UpdateUser<T>(Guid id, User user)
        {
            try
            {
                var existingUser = await _dbContext.Users.FindAsync(id);
                if (existingUser is null)
                    return (T)_appResponse.BadRequest("User does not exist");

                existingUser.first_name = user.first_name;
                existingUser.last_name = user.last_name;
                existingUser.password = user.password;

                var updatedUser = _dbContext.Users.Update(existingUser);
                await _dbContext.SaveChangesAsync();

                var responseData = await _dbContext.Users.FindAsync(updatedUser.Entity.Id);
                if (responseData is null)
                    return (T)_appResponse.BadRequest("Could not fetch user");
                return (T)_appResponse.Ok(responseData, "User Updated");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        public async Task<T> DeleteUser<T>(Guid id)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(id);
                if (user is null)
                    return (T)_appResponse.BadRequest("User Does Not Exist");
                var removedUser = _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                return (T)_appResponse.Ok(removedUser.Entity.Id, "User Deleted");
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }
    }
}
