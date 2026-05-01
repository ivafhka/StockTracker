using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StockTracker.Application.Behaviors;
using StockTracker.Application.Services;
using System.Reflection;

namespace StockTracker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(assembly);

            services.AddScoped<IAlertService, AlertService>();
            services.AddSingleton<IPortfolioValuationService, PortfolioValuationService>();
            return services;
        }
    }
}
