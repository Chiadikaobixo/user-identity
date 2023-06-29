using AppResponse;
using Db;
using UserEntity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UserDTO;
using Hash;
using AutoMapper;

namespace Services
{
    public class UserService
    {
        private readonly DatabaseContext _dbContext;
        private readonly Response _appResponse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Hashed _hashed;
        // private readonly IMapper _mapper;

        public UserService(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor, Response appResponse, Hashed hashed
        // IMapper mapper
        )
        {
            _dbContext = dbContext;
            _appResponse = appResponse;
            _httpContextAccessor = httpContextAccessor;
            _hashed = hashed;
            // _mapper = mapper;
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
                    return (T)_appResponse.BadRequest("User does not exist@@");

                if (!string.IsNullOrEmpty(updateUser.first_name))
                    existingUser.first_name = updateUser.first_name;
                if (!string.IsNullOrEmpty(updateUser.last_name))
                    existingUser.last_name = updateUser.last_name;
                if (!string.IsNullOrEmpty(updateUser.password))
                    existingUser.password = _hashed.hashedPassword(updateUser.password);
                if (!string.IsNullOrEmpty(updateUser.date_of_birth))
                    existingUser.date_of_birth = updateUser.date_of_birth;

                // var updateProperties = _mapper.Map(updateUser, existingUser);

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
                var userId = UserIdClaim();
                var user = await _dbContext.Users.FindAsync(userId);
                if (user is null)
                    return (T)_appResponse.BadRequest("User Does Not Exist");
                
                if (user.Role == UserRole.Admin)
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

        // internal class MappingProfile : Profile
        // {
        //     public MappingProfile()
        //     {
        //         CreateMap<UserUpdateDTO, User>();
        //     }
        // }
    }
}
