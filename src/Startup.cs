using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
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
using Wallet_service;
using User_Claim;
using Transaction_service;
using Order_service;
using Transaction_helper;
using paystack_charge;
using System;

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
            var paystackSecretKey = Configuration.GetSection("Paystack:SecretKey").Value;
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.AddCors();

            services.AddScoped<AuthServices>();
            services.AddScoped<UserService>();
            services.AddScoped<Response>();
            services.AddScoped<Hashed>();
            services.AddScoped<Token>();
            services.AddScoped<WalletService>();
            services.AddScoped<TransactionService>();
            services.AddScoped<ClaimService>();
            services.AddScoped<OrderService>();
            services.AddScoped<TransactionHelper>();
            services.AddScoped<PaystackCharge>();
            services.AddHttpContextAccessor();
            services.AddSingleton<PaystackCharge>(provider => new PaystackCharge(paystackSecretKey));
            // services.AddTransient<PaystackCharge>(provider => new PaystackCharge(paystackSecretKey));

            // Configure JWT authentication
            var jwtSecretKey = Configuration["Jwt:SecretKey"];
            var jwtIssuer = Configuration["Jwt:ValidIssuer"];
            var jwtAudience = Configuration["Jwt:ValidAudience"];
            var issuerSecretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey ?? "hhhhh"));

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
                // Add the security definition
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "User Identity JWT Authorization header using the Bearer scheme",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
                // Add the security requirement
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                            new List<string>()
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();

            // Use authentication middleware
            app.UseAuthentication();

            // Use authorization middleware
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable Swagger UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Identity API V1");
                c.RoutePrefix = string.Empty;
            });

            // Enable Swagger JSON endpoint
            app.UseSwagger();

            // Use the JwtTokenValidationMiddleware only for endpoints other than CreateUser and Login
            app.MapWhen(context =>
            {
                return !context.Request.Path.StartsWithSegments("/api/auth/create") &&
                       !context.Request.Path.StartsWithSegments("/api/auth/login");
            }, builder =>
            {
                builder.UseMiddleware<JwtTokenValidationMiddleware>();
            });
        }
    }
}
