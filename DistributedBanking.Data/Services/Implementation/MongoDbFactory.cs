using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace DistributedBanking.Data.Services.Implementation;

public class MongoDbFactory : IMongoDbFactory
{
    private readonly string _databaseName;
    private readonly IMongoClient _client;

    public MongoDbFactory(string connectionString, string databaseName)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        settings.ClusterConfigurator = cb => { 
            cb.Subscribe<CommandStartedEvent>(e => 
            {
                new Logger<MongoDbFactory>(MongoDbDriverLoggerFactory.LoggerFactory)
                    .LogInformation("{CommandName} - {CommandJson}", e.CommandName, e.Command.ToJson());
            });
        };
        
        _databaseName = databaseName;
        _client = new MongoClient(settings);
    }
    
    public IMongoDatabase GetDatabase()
    {
        return _client.GetDatabase(_databaseName);
    }

   /* public IMongoCollection<T> GetCollection<T>(string collectionNme)
    {
        return _client.GetDatabase(_databaseName).GetCollection<T>(collectionNme);
    }*/
}