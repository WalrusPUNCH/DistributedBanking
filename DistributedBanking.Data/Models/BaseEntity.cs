using MongoDB.Bson.Serialization.Attributes;

namespace DistributedBanking.Data.Models;

public abstract class BaseEntity
{
    [BsonId]
    public Guid Id { get; set; }
}