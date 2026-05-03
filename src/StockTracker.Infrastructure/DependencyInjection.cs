
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockTracker.Application.Interfaces;
using StockTracker.Domain.Interfaces;
using StockTracker.Infrastructure.Kafka.Configuration;
using StockTracker.Infrastructure.Kafka.Producer;
using StockTracker.Infrastructure.Persistence;
using StockTracker.Infrastructure.Persistence.Repositories;
using StockTracker.Infrastructure.Security;

namespace StockTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' is not configured");

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                }));

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPortfolioRepository, PortfolioRepository>();
            services.AddScoped<IAlertRepository, AlertRepository>();
            services.AddScoped<IPriceTickRepository, PriceTickRepository>();

            services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

            services.Configure<KafkaOptions>(configuration.GetSection(KafkaOptions.SectionName));
            services.AddSingleton<ITickProducer, TickProducer>();
            services.AddSingleton<IEventBus, KafkaEventBus>();
            
            return services;
        }
    }
}
