using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Data.Models.EndUsers;
using DistributedBanking.Data.Repositories.Implementation.TransactionalClock.Base;
using DistributedBanking.Data.Services;
using MongoDB.Driver;
using TransactionalClock.Integration;

namespace DistributedBanking.Data.Repositories.Implementation.TransactionalClock;

public class WorkersTcRepository : RepositoryTcBase<WorkerEntity>, IWorkersRepository
{
    private IMongoDatabase _database;
    
    public WorkersTcRepository(
        IMongoDbFactory mongoDbFactory,
        ITransactionalClockClient transactionalClockClient) 
        : base(transactionalClockClient, mongoDbFactory.GetDatabase(), CollectionNames.EndUsers)
    {
        _database = mongoDbFactory.GetDatabase();
    }
}