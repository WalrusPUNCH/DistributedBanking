using DistributedBanking.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace DistributedBanking.Data.Repositories.Implementation;

public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
{
    protected readonly IMongoCollection<T> Collection;
    protected readonly FilterDefinitionBuilder<T> FilterBuilder = Builders<T>.Filter;
    private readonly MongoCollectionSettings _mongoCollectionSettings = new() { GuidRepresentation = GuidRepresentation.Standard };
    
    protected RepositoryBase(IMongoDatabase database, string collectionName)
    {
        Collection = database.GetCollection<T>(collectionName, _mongoCollectionSettings);
    }

    public virtual async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await Collection.Find(FilterBuilder.Empty).ToListAsync();
    }

    public virtual async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        return await Collection.Find(filter).ToListAsync();
    }

    public virtual async Task<T> GetAsync(Guid id)
    {
        var filter = FilterBuilder.Eq(e => e.Id, id);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? filter)
    {
        return await Collection.Find(filter ?? FilterDefinition<T>.Empty).ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        await Collection.InsertOneAsync(entity);
    }

    public virtual async Task UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        FilterDefinition<T> filter = FilterBuilder.Eq(e => e.Id, entity.Id);
        await Collection.ReplaceOneAsync(filter, entity);
    }

    public virtual async Task RemoveAsync(Guid id)
    {
        FilterDefinition<T> filter = FilterBuilder.Eq(e => e.Id, id);
        await Collection.DeleteOneAsync(filter);
    }
}