using DistributedBanking.Data.Models;
using DistributedBanking.Data.Repositories.Implementation.Default.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using TransactionalClock.Integration;

namespace DistributedBanking.Data.Repositories.Implementation.TransactionalClock.Base;

public class RepositoryTcBase<T> :  RepositoryBase<T> where T : BaseEntity
{
    private readonly string _databaseName;
    private readonly string _collectionName;
    private readonly ITransactionalClockClient _transactionalClockClient;
    
    protected RepositoryTcBase(
        ITransactionalClockClient transactionalClockClient,
        IMongoDatabase database, 
        string collectionName) : base(database, collectionName)
    {
        _databaseName = database.DatabaseNamespace.DatabaseName;
        _collectionName = collectionName;
        
        _transactionalClockClient = transactionalClockClient;
    }
    
    public override async Task AddAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var transactionalClockResponse = await _transactionalClockClient.Create(
            database: _databaseName,
            collection: _collectionName,
            entity);

        entity.Id = transactionalClockResponse.Id;
    }

    public override async Task UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        
        var response = await _transactionalClockClient.Update(
            id: entity.Id.ToString(),
            database: _databaseName,
            collection: _collectionName,
            createdAt: DateTime.UtcNow.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"),
            entity);

        var x = 10;
    }

    public override async Task RemoveAsync(ObjectId id)
    {
        var response = await _transactionalClockClient.Delete(
            id: id.ToString(),
            database: _databaseName,
            collection: _collectionName);

        var x = 10;
    }
}