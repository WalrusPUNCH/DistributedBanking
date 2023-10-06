using DistributedBanking.Data.Models;
using DistributedBanking.Data.Services;
using MongoDB.Driver;

namespace DistributedBanking.Data.Repositories.Implementation;

public class CustomersRepository : RepositoryBase<CustomerEntity>, ICustomersRepository
{
    private IMongoDatabase _database;
    
    public CustomersRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory.GetDatabase(), CollectionNames.Customers)
    {
        _database = mongoDbFactory.GetDatabase();
    }
}