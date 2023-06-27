using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace Middleware
{
    public class JwtTokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtTokenValidationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecretKey = _configuration["Jwt:SecretKey"];
                var jwtIssuer = _configuration["Jwt:ValidIssuer"];
                var jwtAudience = _configuration["Jwt:ValidAudience"];

                try
                {
                    var issuerSecretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey?? "hhhhh"));
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = issuerSecretKey,
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = jwtAudience,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero

                    };

                    var result = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                    context.User = result;
                }
                catch (Exception)
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }

            await _next(context);
        }
    }

    public static class JwtTokenValidationMiddlewareExtensions
    {
        public static IServiceCollection AddJwtTokenValidation(this IServiceCollection services)
        {
            services.AddTransient<JwtTokenValidationMiddleware>();

            return services;
        }
    }
}
