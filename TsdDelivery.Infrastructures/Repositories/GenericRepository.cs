﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;
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
    public async Task<TEntity?> AddAsync(TEntity entity)
    {
        entity.CreationDate = _timeService.GetCurrentTime();
        entity.CreatedBy = _claimsServce.GetCurrentUserId;
        await _dbSet.AddAsync(entity);
        return entity;
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

    public Task<List<TEntity>> GetAllAsync(string[] includes = null)
    {
        if(includes != null && includes.Count() > 0)
        {
            var query = _dbSet.Include(includes.First());
            foreach (var include  in includes.Skip(1))
                query = query.Include(include);
            return query.AsQueryable().ToListAsync();
        }
        return _dbSet.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        var result = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        // todo should throw exception when not found
        if (result == null)
            throw new Exception($"Not Found by ID: [{id}] of [{typeof(TEntity).Name}]");
        return result;
    }
    
    public async Task SoftRemove(TEntity entity)
    {
        entity.IsDeleted = true;
        entity.DeleteBy = _claimsServce.GetCurrentUserId;
        _dbSet.Update(entity);
    }

    public async Task SoftRemoveRange(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletionDate = _timeService.GetCurrentTime();
            entity.DeleteBy = _claimsServce.GetCurrentUserId;
        }
        _dbSet.UpdateRange(entities);
    }

    public Task<int> Count()
    {
        return _dbSet.CountAsync();
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

    public async Task Update(TEntity entity)
    {
        entity.ModificationDate = _timeService.GetCurrentTime();
        entity.ModificationBy = _claimsServce.GetCurrentUserId;
        _dbSet.Update(entity);
    }

    public async Task UpdateRange(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.CreationDate = _timeService.GetCurrentTime();
            entity.CreatedBy = _claimsServce.GetCurrentUserId;
        }
        _dbSet.UpdateRange(entities);
    }

    public async Task Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public Task<TEntity> GetSingleByCondition(Expression<Func<TEntity, bool>> expression, string[] includes = null)
    {
        if (includes != null && includes.Count() > 0)
        {
            var query = _dbSet.Include(includes.First());
            foreach (var include in includes.Skip(1))
                query = query.Include(include);
            return query.FirstAsync(expression);
        }
        return _dbSet.FirstAsync(expression);
    }
    
    public Task<List<TEntity>> GetMulti(Expression<Func<TEntity, bool>> predicate, string[] includes = null)
    {
        //HANDLE INCLUDES FOR ASSOCIATED OBJECTS IF APPLICABLE
        if (includes != null && includes.Count() > 0)
        {
            var query = _dbSet.Include(includes.First());
            foreach (var include in includes.Skip(1))
                query = query.Include(include);
            return query.Where(predicate).ToListAsync();
        }
        return _dbSet.Where(predicate).ToListAsync();
    }
}
