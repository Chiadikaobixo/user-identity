using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace User_Claim
{
    public class ClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid? AuthenticatedUserClaim()
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

        public string? AuthenticatedEmailClaim()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null)
            {
                var emailIdentifierClaim = user.FindFirst(ClaimTypes.Email);

                if (emailIdentifierClaim != null)
                {
                    return emailIdentifierClaim.Value;
                }
            }
            return null;
        }
    }
}