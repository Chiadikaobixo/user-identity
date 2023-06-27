using AppResponse;
using Db;
using UserEntity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UserDTO;

namespace Services
{
    public class UserService
    {
        private readonly DatabaseContext _dbContext;

        private readonly Response _appResponse;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor, Response appResponse)
        {
            _dbContext = dbContext;
            _appResponse = appResponse;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<T> UpdateUser<T>(UserUpdateDTO updateUser)
        {
            try
            {
                var userId = UserIdClaim();
                var existingUser = await _dbContext.Users.FindAsync(userId);
                if (existingUser is null)
                    return (T)_appResponse.BadRequest("User does not exist");

                existingUser.first_name = updateUser.first_name;
                existingUser.last_name = updateUser.last_name;
                existingUser.password = updateUser.password;

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

        public async Task<T> DeleteUser<T>()
        {
            try
            {
                var userId = UserIdClaim();
                var user = await _dbContext.Users.FindAsync(userId);
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

        public Guid? UserIdClaim()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null)
            {
                var nameIdentifierClaim = user.FindFirst(ClaimTypes.NameIdentifier);

                if (nameIdentifierClaim != null)
                {
                    if (Guid.TryParse(nameIdentifierClaim.Value, out Guid nameIdentifierValue))
                    {
                        return nameIdentifierValue;
                    }
                }
            }
            return null;
        }
    }
}
