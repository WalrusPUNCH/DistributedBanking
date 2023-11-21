using DistributedBanking.Data.Models;
using DistributedBanking.Data.Repositories.Implementation.Default.Base;

namespace DistributedBanking.Data.Repositories;

public interface ITransactionsRepository : IRepositoryBase<TransactionEntity>
{
    Task<IEnumerable<TransactionEntity>> AccountTransactionHistory(string accountId);
}