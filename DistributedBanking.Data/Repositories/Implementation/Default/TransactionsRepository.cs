﻿using DistributedBanking.Data.Models;
using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Data.Repositories.Implementation.Default.Base;
using DistributedBanking.Data.Services;
using MongoDB.Driver;

namespace DistributedBanking.Data.Repositories.Implementation.Default;

public class TransactionsRepository : RepositoryBase<TransactionEntity>, ITransactionsRepository
{
    private IMongoDatabase _database;
    
    public TransactionsRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory.GetDatabase(), CollectionNames.Transactions)
    {
        _database = mongoDbFactory.GetDatabase();
    }

    public async Task<IEnumerable<TransactionEntity>> AccountTransactionHistory(string accountId)
    {
        return await Collection
            .Find(t => t.SourceAccountId == accountId || t.DestinationAccountId == accountId)
            .SortByDescending(t => t.DateTime)
            .ToListAsync();
    }
}