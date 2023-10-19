using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Data.Models.EndUsers;
using DistributedBanking.Data.Services;
using MongoDB.Driver;

namespace DistributedBanking.Data.Repositories.Implementation;

public class CustomersRepository : RepositoryBase<CustomerEntity>, ICustomersRepository
{
    private IMongoDatabase _database;
    
    public CustomersRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory.GetDatabase(), CollectionNames.EndUsers)
    {
        _database = mongoDbFactory.GetDatabase();
    }
}