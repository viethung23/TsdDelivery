using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    public Task<User?> GetUserByPhoneNumber(string phoneNumber);
}
