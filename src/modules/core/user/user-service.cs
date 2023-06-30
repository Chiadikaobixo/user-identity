using AppResponse;
using Db;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UserDTO;
using Hash;
using User_Claim;

namespace Services
{
    public class UserService
    {
        private readonly DatabaseContext _dbContext;
        private readonly Response _appResponse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimService _claimService;
        private readonly Hashed _hashed;

        public UserService(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor, ClaimService claimService, Response appResponse, Hashed hashed)
        {
            _dbContext = dbContext;
            _appResponse = appResponse;
            _claimService = claimService;
            _httpContextAccessor = httpContextAccessor;
            _hashed = hashed;
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
                var userId =  _claimService.AuthenticatedUserClaim();

                var existingUser = await _dbContext.Users.FindAsync(userId);
                if (existingUser is null)
                    return (T)_appResponse.BadRequest("User does not exist@@");

                if (!string.IsNullOrEmpty(updateUser.first_name))
                    existingUser.first_name = updateUser.first_name;
                if (!string.IsNullOrEmpty(updateUser.last_name))
                    existingUser.last_name = updateUser.last_name;
                if (!string.IsNullOrEmpty(updateUser.password))
                    existingUser.password = _hashed.hashedPassword(updateUser.password);
                if (!string.IsNullOrEmpty(updateUser.date_of_birth))
                    existingUser.date_of_birth = updateUser.date_of_birth;

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

        public async Task<T> DeleteUser<T>(Guid? optionalId = null)
        {
            try
            {
                var userId =  _claimService.AuthenticatedUserClaim();

                var user = await _dbContext.Users.FindAsync(userId);
                if (user is null)
                    return (T)_appResponse.BadRequest("User Does Not Exist");
                
                if (user.role == UserRole.Admin)
                {
                    var deleteUserId = optionalId ?? userId;
                    var userToDelete = await _dbContext.Users.FindAsync(deleteUserId);
                    if (userToDelete is null)
                        return (T)_appResponse.BadRequest("User Does Not Exist");

                    var deletedUser = _dbContext.Users.Remove(userToDelete);
                    await _dbContext.SaveChangesAsync();

                    return (T)_appResponse.Ok(deletedUser.Entity.Id, "User Deleted");
                }
                
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
