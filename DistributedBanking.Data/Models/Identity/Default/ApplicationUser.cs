using AspNetCore.Identity.MongoDbCore.Models;
using DistributedBanking.Data.Models.Constants;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;

namespace DistributedBanking.Data.Models.Identity.Default;

[CollectionName(CollectionNames.Service.Users)]
public class ApplicationUser : MongoIdentityUser<ObjectId>
{
    public string EndUserId { get; set; }
}