using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Data.Models.Identity.TransactionalClock;
using DistributedBanking.Data.Repositories.Implementation.TransactionalClock.Base;
using DistributedBanking.Data.Services;
using MongoDB.Driver;
using TransactionalClock.Integration;

namespace DistributedBanking.Data.Repositories.Implementation.TransactionalClock;

public class RolesTcRepository : RepositoryTcBase<ApplicationTcRole>, IRolesTcRepository
{
    private IMongoDatabase _database;
    
    public RolesTcRepository(
        IMongoDbFactory mongoDbFactory,
        ITransactionalClockClient transactionalClockClient) 
        : base(transactionalClockClient, mongoDbFactory.GetDatabase(), CollectionNames.Service.Roles)
    {
        _database = mongoDbFactory.GetDatabase();
    }
}