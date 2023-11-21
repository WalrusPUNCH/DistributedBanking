using MongoDB.Bson.Serialization.Attributes;

namespace DistributedBanking.Data.Models.Identity.TransactionalClock;

public class ApplicationTcRole : BaseEntity
{
    [BsonElement(nameof(Name))]
    public string Name { get; init; }
    
    [BsonElement(nameof(NormalizedName))]
    public string NormalizedName { get; init; }
    
    public ApplicationTcRole(string name)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
    }
}