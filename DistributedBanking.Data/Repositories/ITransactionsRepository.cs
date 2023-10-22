using DistributedBanking.Data.Models;

namespace DistributedBanking.Data.Repositories;

public interface ITransactionsRepository : IRepositoryBase<TransactionEntity>
{
    Task<IEnumerable<TransactionEntity>> AccountTransactionHistory(Guid accountId);
}