using DistributedBanking.Data.Models;
using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Data.Repositories.Implementation.Default.Base;
using DistributedBanking.Data.Services;
using MongoDB.Driver;

namespace DistributedBanking.Data.Repositories.Implementation.Default;

public class AccountsRepository : RepositoryBase<AccountEntity>, IAccountsRepository
{
    private IMongoDatabase _database;
    
    public AccountsRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory.GetDatabase(), CollectionNames.Accounts)
    {
        _database = mongoDbFactory.GetDatabase();
    }
}