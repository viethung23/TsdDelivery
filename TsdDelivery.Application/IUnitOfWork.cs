using Microsoft.EntityFrameworkCore.Storage;
using TsdDelivery.Application.Repositories;

namespace TsdDelivery.Application;

public interface IUnitOfWork : IDisposable
{
    public IUserRepository UserRepository { get; }
    public IRoleRepository RoleRepository { get; }
    public IVehicleTypeReposiory VehicleTypeReposiory { get; }
    public IVehicleRepository VehicleRepository { get; }
    public IServiceRepository ServiceRepository { get; }
    public IShippingRateRepository ShippingRateRepository { get; }

    public Task<int> SaveChangeAsync();
    IDbContextTransaction BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync();
}
