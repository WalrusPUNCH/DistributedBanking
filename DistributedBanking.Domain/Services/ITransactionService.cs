using DistributedBanking.Domain.Models.Transaction;

namespace DistributedBanking.Domain.Services;

public interface ITransactionService
{
    Task<TransactionStatusModel> Deposit(OneWayTransactionModel depositTransactionModel);
    Task<TransactionStatusModel> Withdraw(OneWaySecuredTransactionModel withdrawTransactionModel);
    Task<TransactionStatusModel> Transfer(TwoWayTransactionModel transferTransactionModel);
    Task<decimal> GetBalance(Guid accountId);
    Task<IEnumerable<TransactionResponseModel>> GetAccountTransactionHistory(Guid accountId);
}