using Microsoft.EntityFrameworkCore.Storage;
using TsdDelivery.Application;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Infrastructures.Repositories;

namespace TsdDelivery.Infrastructures;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _appDbContext;
    private readonly ICurrentTime _currentTime;
    private readonly IClaimsService _claimsService;
    private bool disposed = false;
    public UnitOfWork(AppDbContext appDbContext, IClaimsService claimsService, ICurrentTime currentTime)
    {
        _appDbContext = appDbContext;
        _claimsService = claimsService;
        _currentTime = currentTime;
    }
    public IUserRepository UserRepository => new UserRepository(_appDbContext,_currentTime,_claimsService);

    public IRoleRepository RoleRepository => new RoleRepository(_appDbContext,_currentTime,_claimsService);

    public IVehicleTypeReposiory VehicleTypeReposiory => new VehicleTypeRepository(_appDbContext, _claimsService, _currentTime);

    public IVehicleRepository VehicleRepository => new VehicleRepository(_appDbContext,_currentTime,_claimsService);
    public IServiceRepository ServiceRepository => new ServiceRepository(_appDbContext, _currentTime, _claimsService);

    public IShippingRateRepository ShippingRateRepository => new ShippingRateRepository(_appDbContext, _currentTime, _claimsService);

    public IReservationRepository ReservationRepository => new ReservationRepository(_appDbContext, _claimsService, _currentTime);

    public IReservationDetailRepository ReservationDetailRepository => new ReservationDetailRepository(_appDbContext, _claimsService, _currentTime);

    public async Task<int> SaveChangeAsync()
    {
        return await _appDbContext.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _appDbContext.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IDbContextTransaction BeginTransaction()
    {
        return _appDbContext.Database.BeginTransaction();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _appDbContext.Database.BeginTransactionAsync();
    }
}
