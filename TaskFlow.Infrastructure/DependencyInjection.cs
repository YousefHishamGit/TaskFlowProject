using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Infrastructure.Caching;
using TaskFlow.Infrastructure.Identity;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
           
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        
            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<AppDbContext>());

            var redisConnectionString = configuration.GetConnectionString("Redis");
            if (string.IsNullOrEmpty(redisConnectionString))
            {
                throw new InvalidOperationException("Redis connection string 'Redis' not found.");
            }
            services.AddStackExchangeRedisCache(options =>
            {
                var configOptions = ConfigurationOptions.Parse(redisConnectionString);
                configOptions.AbortOnConnectFail = false; 
                configOptions.ConnectTimeout = 1000;      
                configOptions.AsyncTimeout = 1000;       

                options.ConfigurationOptions = configOptions;
                options.InstanceName = "TaskFlow:";       
            });


            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddSingleton<ICacheService, CacheService>();

            return services;
        }
    }
}

