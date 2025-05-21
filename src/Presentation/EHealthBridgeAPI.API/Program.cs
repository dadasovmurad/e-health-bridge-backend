using EHealthBridgeAPI.Persistence;
using EHealthBridgeAPI.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using EHealthBridgeAPI.API.Extensions;
using EHealthBridgeAPI.Application.Features.Profiles;

namespace EHealthBridgeAPI.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var services = builder.Services;

            services.AddControllers();
            services.AddPersistenceServices();
            services.AddInfrastructureServices();
            builder.Services.AddSwaggerWithJwt();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Admin", options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateAudience = true, //Oluþturulacak token deðerini kimlerin/hangi originlerin/sitelerin kullanýcý belirlediðimiz deðerdir. -> www.bilmemne.com
                    ValidateIssuer = true, //Oluþturulacak token deðerini kimin daðýttýný ifade edeceðimiz alandýr. -> www.myapi.com
                    ValidateLifetime = true, //Oluþturulan token deðerinin süresini kontrol edecek olan doðrulamadýr.
                    ValidateIssuerSigningKey = true, //Üretilecek token deðerinin uygulamamýza ait bir deðer olduðunu ifade eden suciry key verisinin doðrulanmasýdýr.

                    ValidAudience = builder.Configuration["Token:Audience"],
                    ValidIssuer = builder.Configuration["Token:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
                    LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

                    NameClaimType = ClaimTypes.Name //JWT üzerinde Name claimne karþýlýk gelen deðeri User.Identity.Name propertysinden elde edebiliriz.
                };
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            //global exception middleware
            app.UseGlobalExceptionMiddleware();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(); // optional: add config
            }

            // app.UseHttpsRedirection(); // optional, for HTTP to HTTPS


           
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
