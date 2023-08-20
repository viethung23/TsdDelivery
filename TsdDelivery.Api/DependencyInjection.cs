using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Services;

namespace TsdDelivery.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIService(this IServiceCollection services)
    {
        services.AddControllers(config =>
        {
            config.Filters.Add(typeof(TsdDeliveryExceptionHandler));
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        //services.AddHealthChecks();
        //services.AddSingleton<GlobalExceptionMiddleware>();
        //services.AddSingleton<PerformanceMiddleware>();
        //services.AddSingleton<Stopwatch>();
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddHttpContextAccessor();
        /*services.AddScoped<ValidateGuidAttribute>();
        services.AddScoped<ValidateModelAttribute>();*/
        //services.AddFluentValidationAutoValidation();
        //services.AddFluentValidationClientsideAdapters();

        //services.AddAutoMapper(typeof(UserMapConfig).Assembly);
        return services;
    }
}
