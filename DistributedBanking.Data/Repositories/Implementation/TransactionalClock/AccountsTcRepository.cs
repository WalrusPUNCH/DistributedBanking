using DistributedBanking.Data.Models;
using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Data.Repositories.Implementation.TransactionalClock.Base;
using DistributedBanking.Data.Services;
using MongoDB.Driver;
using TransactionalClock.Integration;

namespace DistributedBanking.Data.Repositories.Implementation.TransactionalClock;

public class AccountsTcRepository : RepositoryTcBase<AccountEntity>, IAccountsRepository
{
    private IMongoDatabase _database;
    
    public AccountsTcRepository(
        IMongoDbFactory mongoDbFactory,
        ITransactionalClockClient transactionalClockClient) 
            : base(transactionalClockClient, mongoDbFactory.GetDatabase(), CollectionNames.Accounts)
    {
        _database = mongoDbFactory.GetDatabase();
    }
}
