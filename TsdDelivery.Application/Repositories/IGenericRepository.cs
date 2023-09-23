using System.Linq.Expressions;
using TsdDelivery.Application.Commons;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task<List<TEntity>> GetAllAsync(string[] includes = null);
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<TEntity?> AddAsync(TEntity entity);
    Task Update(TEntity entity);
    Task UpdateRange(List<TEntity> entities);
    Task SoftRemove(TEntity entity);
    Task Delete(TEntity entity);
    Task AddRangeAsync(List<TEntity> entities);
    Task SoftRemoveRange(List<TEntity> entities);

    Task<TEntity> GetSingleByCondition(Expression<Func<TEntity, bool>> expression, string[] includes = null);
    Task<List<TEntity>> GetMulti(Expression<Func<TEntity, bool>> predicate, string[] includes = null);
    Task<Pagination<TEntity>> ToPagination(int pageNumber = 0, int pageSize = 10);
}
