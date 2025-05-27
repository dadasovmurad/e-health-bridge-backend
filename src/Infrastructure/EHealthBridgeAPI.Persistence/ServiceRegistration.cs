using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Persistence.Contexts.Dapper;
using EHealthBridgeAPI.Persistence.Repositories;
using EHealthBridgeAPI.Persistence.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddSingleton<EHealthBridgeAPIDbContext>();
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IAuthService,AuthService>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();
            //services.AddScoped<IGenericRepository, GenericRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
