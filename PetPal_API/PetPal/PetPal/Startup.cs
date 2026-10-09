using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetPal;
using PetPal.Middlewares;
using PetPal.Models;
using PetPal.Repository;
using PetPal.Repository.Interfaces;
using PetPal.Services;
using PetPal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetPal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddHttpContextAccessor();

            services.Configure<ConnectionString>(
                Configuration.GetSection("ConnectionStrings")
            );

            services.Configure<JwtSettings>(
                Configuration.GetSection("Jwt")
            );

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IPetService, PetService>();
            services.AddScoped<IPetRepository, PetRepository>();

            services.AddScoped<IAdoptionRequestRepository, AdoptionRequestRepository>();
            services.AddScoped<IAdoptionRequestService, AdoptionRequestService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                Configuration["Jwt:Key"])),

                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = Configuration["Jwt:Audience"],

                        ValidateLifetime = true
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var response = new
                            {
                                StatusCode = 401,
                                Message = "You are not authenticated. Please login first."
                            };

                            return context.Response.WriteAsJsonAsync(response);
                        },

                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            var response = new
                            {
                                StatusCode = 403,
                                Message = "You do not have permission to access this resource."
                            };

                            return context.Response.WriteAsJsonAsync(response);
                        }
                    };
                });

            services.AddAuthorization();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("PetPal API is running");
                });

                endpoints.MapControllers();
            });
        }
    }
}
