using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace DistributedBanking.Data.Models.Identity;

[CollectionName(CollectionNames.Roles)]
public class ApplicationRole : MongoIdentityRole<Guid>
{
    
}