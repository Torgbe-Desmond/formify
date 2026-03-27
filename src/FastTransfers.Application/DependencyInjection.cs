using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FastTransfers.Application.Common.Behaviors;

namespace FastTransfers.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        //services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Pipeline behaviors — run in order for every MediatR request
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}
