using Microsoft.EntityFrameworkCore;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    protected DbSet<TEntity> _dbSet;
    private readonly ICurrentTime _timeService;
    private readonly IClaimsService _claimsServce;

    public GenericRepository(AppDbContext context, IClaimsService claimsService
        , ICurrentTime currentTime)
    {
        _dbSet = context.Set<TEntity>();
        _claimsServce = claimsService;
        _timeService = currentTime;
    }
    public async Task AddAsync(TEntity entity)
    {
        entity.CreationDate = _timeService.GetCurrentTime();
        entity.CreatedBy = _claimsServce.GetCurrentUserId;
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.CreationDate = _timeService.GetCurrentTime();
            entity.CreatedBy = _claimsServce.GetCurrentUserId;
        }
        await _dbSet.AddRangeAsync(entities);
    }

    public Task<List<TEntity>> GetAllAsync() => _dbSet.ToListAsync();

    public Task<TEntity?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public void SoftRemove(TEntity entity)
    {
        entity.IsDeleted = true;
        entity.DeleteBy = _claimsServce.GetCurrentUserId;
        _dbSet.Update(entity);
    }

    public void SoftRemoveRange(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletionDate = _timeService.GetCurrentTime();
            entity.DeleteBy = _claimsServce.GetCurrentUserId;
        }
        _dbSet.UpdateRange(entities);
    }

    public async Task<Pagination<TEntity>> ToPagination(int pageNumber = 0, int pageSize = 10)
    {
        var itemCount = await _dbSet.CountAsync();
        var items = await _dbSet.OrderByDescending(x => x.CreationDate)
                                .Skip(pageNumber * pageSize)
                                .Take(pageSize)
                                .AsNoTracking()
                                .ToListAsync();

        var result = new Pagination<TEntity>()
        {
            PageIndex = pageNumber,
            PageSize = pageSize,
            TotalItemsCount = itemCount,
            Items = items,
        };

        return result;
    }

    public void Update(TEntity entity)
    {
        entity.ModificationDate = _timeService.GetCurrentTime();
        entity.ModificationBy = _claimsServce.GetCurrentUserId;
        _dbSet.Update(entity);
    }

    public void UpdateRange(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.CreationDate = _timeService.GetCurrentTime();
            entity.CreatedBy = _claimsServce.GetCurrentUserId;
        }
        _dbSet.UpdateRange(entities);
    }
}
