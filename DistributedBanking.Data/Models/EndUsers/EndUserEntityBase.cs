using MongoDB.Bson.Serialization.Attributes;

namespace DistributedBanking.Data.Models.EndUsers;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(CustomerEntity), typeof(WorkerEntity))]
public abstract class EndUserEntityBase : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime BirthDate { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
}