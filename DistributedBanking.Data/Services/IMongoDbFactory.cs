using MongoDB.Driver;

namespace DistributedBanking.Data.Services;

public interface IMongoDbFactory
{
    IMongoDatabase GetDatabase();
    IMongoCollection<T> GetCollection<T>(string collectionNme);
}
