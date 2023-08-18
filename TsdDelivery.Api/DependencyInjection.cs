using System.Diagnostics;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Services;

namespace TsdDelivery.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIService(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        //services.AddHealthChecks();
        //services.AddSingleton<GlobalExceptionMiddleware>();
        //services.AddSingleton<PerformanceMiddleware>();
        //services.AddSingleton<Stopwatch>();
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddHttpContextAccessor();
        //services.AddFluentValidationAutoValidation();
        //services.AddFluentValidationClientsideAdapters();

        //services.AddAutoMapper(typeof(UserMapConfig).Assembly);
        return services;
    }
}
