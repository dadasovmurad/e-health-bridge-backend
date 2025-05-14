using EHealthBridgeAPI.Persistence.Contexts.Dapper;
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
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
