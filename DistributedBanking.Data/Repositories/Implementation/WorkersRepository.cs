using DistributedBanking.Data.Models;
using DistributedBanking.Data.Models.EndUsers;
using DistributedBanking.Data.Services;
using MongoDB.Driver;

namespace DistributedBanking.Data.Repositories.Implementation;

public class WorkersRepository : RepositoryBase<WorkerEntity>, IWorkersRepository
{
    private IMongoDatabase _database;
    
    public WorkersRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory.GetDatabase(), CollectionNames.EndUsers)
    {
        _database = mongoDbFactory.GetDatabase();
        
       // FilterBuilder = FilterBuilder.Eq()
    }
}