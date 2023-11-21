using System.Text.Json.Serialization;
using DistributedBanking.Data.Services.Implementation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DistributedBanking.Data.Models;

public abstract class BaseEntity
{
    [BsonId]
    [JsonConverter(typeof(ObjectIdJsonConverter)), JsonPropertyName("_id"), BsonElement(nameof(Id))]
    public ObjectId Id { get; set; }
}