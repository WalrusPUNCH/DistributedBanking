using MongoDB.Driver;

namespace DistributedBanking.Data.Services.Implementation;

public class MongoDbFactory : IMongoDbFactory
{
    private readonly string _databaseName;
    private readonly IMongoClient _client;

    public MongoDbFactory(string connectionString, string databaseName)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        _databaseName = databaseName;
        _client = new MongoClient(settings);
    }
    
    public IMongoDatabase GetDatabase()
    {
        return _client.GetDatabase(_databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionNme)
    {
        return _client.GetDatabase(_databaseName).GetCollection<T>(collectionNme);
    }
}