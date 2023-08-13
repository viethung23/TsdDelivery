using Microsoft.Extensions.DependencyInjection;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Application.Services;
using TsdDelivery.Application;
using TsdDelivery.Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TsdDelivery.Infrastructures;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
    {
        //services.AddScoped<IChemicalService, ChemicalService>();
        //services.AddScoped<IChemicalRepository, ChemicalRepository>();
        //services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<ICurrentTime, CurrentTime>();

        // ATTENTION: if you do migration please check file README.md
        services.AddDbContext<AppDbContext>(option => option.UseSqlServer(databaseConnection));

        // this configuration just use in-memory for fast develop
        //services.AddDbContext<AppDbContext>(option => option.UseInMemoryDatabase("test"));

        
        return services;
    }
}
