using TsdDelivery.Application.Repositories;

namespace TsdDelivery.Application;

public interface IUnitOfWork : IDisposable
{
    public IUserRepository UserRepository { get; }

    public Task<int> SaveChangeAsync();
}
