using System.Linq.Expressions;
using DistributedBanking.Data.Models;
using MongoDB.Driver;

namespace DistributedBanking.Data.Repositories.Implementation;

public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
{
    protected readonly IMongoCollection<T> Collection;
    protected readonly FilterDefinitionBuilder<T> FilterBuilder = Builders<T>.Filter;

    protected RepositoryBase(IMongoDatabase database, string collectionName)
    {
        Collection = database.GetCollection<T>(collectionName);
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await Collection.Find(FilterBuilder.Empty).ToListAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<T> GetAsync(Guid id)
    {
        FilterDefinition<T> filter = FilterBuilder.Eq(e => e.Id, id);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? filter)
    {
        return await Collection.Find(filter ?? FilterDefinition<T>.Empty).ToListAsync();
    }

    public async Task CreateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        await Collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        FilterDefinition<T> filter = FilterBuilder.Eq(e => e.Id, entity.Id);
        await Collection.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        FilterDefinition<T> filter = FilterBuilder.Eq(e => e.Id, id);
        await Collection.DeleteOneAsync(filter);
    }
}