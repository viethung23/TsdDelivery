using System.Reflection;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Services;
using TsdDelivery.Application;
using Microsoft.EntityFrameworkCore;
using Mapster;
using MapsterMapper;
using StackExchange.Redis;
using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Application.Services.Momo;
using TsdDelivery.Application.Services.PayPal;
using TsdDelivery.Application.Services.V1;
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

        
        services.AddScoped<IMapService, MapService>();
        services.AddScoped<IDashBoardService, DashBoardService>();
        services.AddTransient<IMailService, MailService>();
        services.AddScoped<IMomoService, MomoService>();
        services.AddScoped<IZaloPayService, ZaloPayService>();
        services.AddScoped<IBlobStorageAzureService, BlobStorageAzureService>();
        services.AddScoped<IPayPalService, PayPalService>();
        services.AddScoped<IPaymentService, PaymentService>();
        
        // add service V1
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IVehicleTypeService,VehicleTypeService>();
        services.AddScoped<IVehicleService,VehicleService>();
        services.AddScoped<IService, ServiceService>();
        services.AddScoped<IShippingRateService, ShippingRateService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ITransactionService, TransactionService>();
        
        // add service V2
        services.AddScoped<Application.Interface.V2.IRoleService, Application.Services.V2.RoleService>();
        services.AddScoped<Application.Interface.V2.IUserService, Application.Services.V2.UserService>();
        services.AddScoped<Application.Interface.V2.IVehicleTypeService, Application.Services.V2.VehicleTypeService>();
        services.AddScoped<Application.Interface.V2.IVehicleService, Application.Services.V2.VehicleService>();
        services.AddScoped<Application.Interface.V2.IService, Application.Services.V2.ServiceService>();
        services.AddScoped<Application.Interface.V2.IWalletService, Application.Services.V2.WalletService>();

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

        //add redis cache
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));



        services.AddHangfireServer();
        services.AddTransient<IBackgroundService, BackgroundService>();
        return services;
    }
}
