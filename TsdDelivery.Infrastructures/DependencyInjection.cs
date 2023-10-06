using System.Reflection;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Services;
using TsdDelivery.Application;
using Microsoft.EntityFrameworkCore;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Application.Services.Momo;
using TsdDelivery.Application.Services.ZaloPay;
using TsdDelivery.Infrastructures.Repositories;

namespace TsdDelivery.Infrastructures;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection,string redisConnection)
    {
        services.AddSingleton<ICurrentTime, CurrentTime>();
        services.AddScoped<IHangFireRepository, HangFireRepository>();
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
        services.AddScoped<IMomoService, MomoService>();
        services.AddScoped<IZaloPayService, ZaloPayService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IMapService, MapService>();

        // ATTENTION: if you do migration please check file README.md
        services.AddDbContext<AppDbContext>(option => option.UseSqlServer(databaseConnection));
        
        // register Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        
        // register hangfire sqlserver 
        services.AddHangfire(hangfire =>
        {
            hangfire.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
            hangfire.UseSimpleAssemblyNameTypeSerializer();
            hangfire.UseRecommendedSerializerSettings();
            hangfire.UseColouredConsoleLogProvider();
            hangfire.UseSqlServerStorage(databaseConnection);
        });
        // hangfire redis
        /*services.AddHangfire(x =>
        {
            x.UseRedisStorage(redisConnection); 
        });*/
        
        
        services.AddHangfireServer();
        services.AddTransient<IBackgroundService, BackgroundService>();
        return services;
    }
}
