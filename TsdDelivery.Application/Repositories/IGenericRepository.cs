using TsdDelivery.Application.Commons;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(Guid id);
    Task AddAsync(TEntity entity);
    Task Update(TEntity entity);
    Task UpdateRange(List<TEntity> entities);
    Task SoftRemove(TEntity entity);
    Task Delete(TEntity entity);
    Task AddRangeAsync(List<TEntity> entities);
    Task SoftRemoveRange(List<TEntity> entities);

    Task<Pagination<TEntity>> ToPagination(int pageNumber = 0, int pageSize = 10);
}
