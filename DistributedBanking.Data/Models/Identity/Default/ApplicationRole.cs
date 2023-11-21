using AspNetCore.Identity.MongoDbCore.Models;
using DistributedBanking.Data.Models.Constants;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;

namespace DistributedBanking.Data.Models.Identity.Default;

[CollectionName(CollectionNames.Service.Roles)]
public class ApplicationRole : MongoIdentityRole<ObjectId>
{
    
}