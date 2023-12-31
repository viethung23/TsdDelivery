﻿using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    public Task<List<User?>> GetUserByPhoneNumber(string phoneNumber);
    public Task<User?> GetUserByPhoneNumberAndRoleId(string phoneNumeber,Guid roleId);
    public Task<User> GetDriverDetail(Guid driverId);
    public Task<User> GetDetail(Guid id);
    public Task<int> GetUserCountByRole(Guid roleId);
}
