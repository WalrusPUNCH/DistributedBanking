using DistributedBanking.Data.Models;
using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Domain.Models.Transaction;

namespace DistributedBanking.Domain.Mapping;

public static class TransactionMapping
{
    public static TransactionEntity AdaptToEntity(this OneWayTransactionModel transactionModel, TransactionType transactionType)
    {
        return new TransactionEntity
        {
            SourceAccountId = transactionModel.SourceAccountId,
            DestinationAccountId = null,
            Type = transactionType,
            Amount = transactionModel.Amount,
            DateTime = DateTime.UtcNow,
            Description = transactionModel.Description
        };
    }
    
    public static TransactionEntity AdaptToEntity(this TwoWayTransactionModel transactionModel, TransactionType transactionType)
    {
        return new TransactionEntity
        {
            SourceAccountId = transactionModel.SourceAccountId,
            DestinationAccountId = transactionModel.DestinationAccountId,
            Type = transactionType,
            Amount = transactionModel.Amount,
            DateTime = DateTime.UtcNow,
            Description = transactionModel.Description
        };
    }
}