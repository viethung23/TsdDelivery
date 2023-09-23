using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Services;
using TsdDelivery.Application;
using Microsoft.EntityFrameworkCore;
using Mapster;
using MapsterMapper;
using TsdDelivery.Infrastructures.Mappers;

namespace TsdDelivery.Infrastructures;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
    {
        services.AddSingleton<ICurrentTime, CurrentTime>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // add service
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IVehicleTypeService,VehicleTypeService>();
        services.AddScoped<IVehicleService,VehicleService>();
        services.AddScoped<IBlobStorageAzureService, BlobStorageAzureService>();
        services.AddScoped<IService, ServiceService>();
        services.AddScoped<IShippingRateService, ShippingRateService>();
        services.AddScoped<IReservationService, ReservationService>();

        // ATTENTION: if you do migration please check file README.md
        services.AddDbContext<AppDbContext>(option => option.UseSqlServer(databaseConnection));
        
        // register Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        
        return services;
    }
}
