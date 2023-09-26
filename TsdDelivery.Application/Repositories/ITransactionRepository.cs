using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Repositories;

public interface ITransactionRepository : IGenericRepository<Transaction>
{
    public Task<List<Transaction>> GetTransactionByUserId(Guid userId);
}