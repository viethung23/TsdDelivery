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
}
