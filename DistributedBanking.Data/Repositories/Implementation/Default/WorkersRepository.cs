using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Data.Models.EndUsers;
using DistributedBanking.Data.Repositories.Implementation.Default.Base;
using DistributedBanking.Data.Services;
using MongoDB.Driver;

namespace DistributedBanking.Data.Repositories.Implementation.Default;

public class WorkersRepository : RepositoryBase<WorkerEntity>, IWorkersRepository
{
    private IMongoDatabase _database;
    
    public WorkersRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory.GetDatabase(), CollectionNames.EndUsers)
    {
        _database = mongoDbFactory.GetDatabase();
     }
}