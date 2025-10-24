using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Wilczura.Demo.Common;

namespace Wilczura.Demo.Persistence.Repositories;

public abstract class ODataRepository<TEntity>
    where TEntity : class, IGetId, new()
{
    protected abstract DbSet<TEntity> DataSet { get; }

    protected ODataRepository()
    {
    }

    protected abstract Expression<Func<TEntity, bool>> GetEntityKeyPredicate(long key);

    protected abstract void AdjustEntityAttributes(TEntity entity, TEntity model);

    protected abstract Task<int> SaveChangesAsync();

    public virtual IQueryable<TEntity> Get(long? id = null)
    {
        IQueryable<TEntity>? query = null;
        if (id.HasValue)
        {
            query = DataSet.Where(GetEntityKeyPredicate(id.Value));
        }
        else
        {
            query = DataSet;
        }

        return query.AsNoTracking();
    }

    public virtual async Task<TEntity?> UpsertAsync(TEntity model)
    {
        TEntity entity;
        if (model.GetId() > 0)
        {
            entity = await DataSet.SingleAsync(GetEntityKeyPredicate(model.GetId()));
        }
        else
        {
            entity = new TEntity();
            DataSet.Add(entity);
        }

        AdjustEntityAttributes(entity, model);

        await SaveChangesAsync();
        TEntity[] savedEntity = [entity];
        var response = savedEntity.AsQueryable().Single();

        return response;
    }
}