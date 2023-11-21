using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Data.Models.Identity.TransactionalClock;
using DistributedBanking.Data.Repositories.Implementation.TransactionalClock.Base;
using DistributedBanking.Data.Services;
using MongoDB.Driver;
using TransactionalClock.Integration;

namespace DistributedBanking.Data.Repositories.Implementation.TransactionalClock;

public class UsersTcRepository : RepositoryTcBase<ApplicationTcUser>, IUsersTcRepository
{
    private IMongoDatabase _database;
    
    public UsersTcRepository(
        IMongoDbFactory mongoDbFactory,
        ITransactionalClockClient transactionalClockClient) 
        : base(transactionalClockClient, mongoDbFactory.GetDatabase(), CollectionNames.Service.Users)
    {
        _database = mongoDbFactory.GetDatabase();
    }

    public async Task<ApplicationTcUser?> GetByEmailAsync(string email)
    {
        return (await GetAsync(u => u.NormalizedEmail == email.NormalizeString())).FirstOrDefault();
    }
}