using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace DistributedBanking.Data.Models.Identity;

[CollectionName(CollectionNames.Service.Users)]
public class ApplicationUser : MongoIdentityUser<Guid>
{
    
}