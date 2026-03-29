using Ecommerce.Domain.Interfaces;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string sqlString, string redisString)
        {
            // Giờ đây ApplicationDbContext đã tồn tại và sẵn sàng được tiêm (Inject)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(sqlString));

            // Đăng ký Redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
