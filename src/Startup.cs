using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Db;
using Auth_Services;
using Services;
using AppResponse;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hash;
using Jwt;
using Middleware;
using Controllers;

namespace Start
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetSection("Options:ConnectionString").Value;
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));

            services.AddScoped<AuthServices>();
            services.AddScoped<UserService>();
            services.AddScoped<Response>();
            services.AddScoped<Hashed>();
            services.AddScoped<Token>();
            services.AddHttpContextAccessor();

            // Configure JWT authentication
            var jwtSecretKey = Configuration["Jwt:SecretKey"];
            var jwtIssuer = Configuration["Jwt:ValidIssuer"];
            var jwtAudience = Configuration["Jwt:ValidAudience"];
            var issuerSecretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey?? "hhhhh"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
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
                });

            // Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Identity", Version = "v1" });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Use authentication middleware
            app.UseAuthentication();

            // Use authorization middleware
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Use the JwtTokenValidationMiddleware only for endpoints other than CreateUser and Login
            app.MapWhen(context =>
            {
                return !context.Request.Path.StartsWithSegments("/api/auth/create") &&
                       !context.Request.Path.StartsWithSegments("/api/auth/login");
            }, builder =>
            {
                builder.UseMiddleware<JwtTokenValidationMiddleware>();
            });

            // Enable Swagger UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Identity API V1");
                c.RoutePrefix = string.Empty;
            });

            // Enable Swagger JSON endpoint
            app.UseSwagger();
        }
    }
}
