using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    public Task<List<User?>> GetUserByPhoneNumber(string phoneNumber);
    public Task<User?> GetUserByPhoneNumberAndRoleId(string phoneNumeber,Guid roleId);
}
