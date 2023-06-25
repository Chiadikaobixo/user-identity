using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserEntity;
using Microsoft.Extensions.Configuration;


namespace Jwt
{
    public class Token
    {
        public IConfiguration Configuration { get; }
        public Token(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string generate(User existingUser)
        {
            var issuerSecretKey = Configuration.GetSection("Jwt:IssuerSigningKey").Value ?? "TTT";
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
                new Claim(ClaimTypes.Email, existingUser.email)
                // Add additional claims as needed
            };
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSecretKey));

            // Generate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1), 
                SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}