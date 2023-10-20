using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class TransactionRepository : GenericRepository<Transaction>,ITransactionRepository
{
    public TransactionRepository(AppDbContext dbContext
        ,ICurrentTime currentTime
        ,IClaimsService claimsService) 
        : base(dbContext,claimsService,currentTime)
    {
        
    }

    public Task<List<Transaction>> GetTransactionByUserId(Guid userId)
    {
        throw new NotImplementedException();
    }
}