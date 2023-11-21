﻿using MongoDB.Bson.Serialization.Attributes;

namespace DistributedBanking.Data.Models.EndUsers;

[BsonIgnoreExtraElements]
public class CustomerEntity : EndUserEntityBase
{
    [BsonElement(nameof(Accounts))]
    public required List<string> Accounts { get; set; } = new ();
    
    [BsonElement(nameof(Passport))]
    public required CustomerPassport Passport { get; set; }
}