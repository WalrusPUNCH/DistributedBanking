using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Data.Repositories;
using DistributedBanking.Domain.Mapping;
using DistributedBanking.Domain.Models.Transaction;
using Mapster;
using Microsoft.Extensions.Logging;

namespace DistributedBanking.Domain.Services.Implementation;

public class TransactionService : ITransactionService
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IAccountsRepository _accountsRepository;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        ITransactionsRepository transactionsRepository, 
        IAccountsRepository accountsRepository,
        ILogger<TransactionService> logger)
    {
        _transactionsRepository = transactionsRepository;
        _accountsRepository = accountsRepository;
        _logger = logger;
    }

    public async Task<TransactionStatusModel> Deposit(OneWayTransactionModel depositTransactionModel)
    {
        try
        {
            var account = await _accountsRepository.GetAsync(depositTransactionModel.SourceAccountId);
            if (!AccountValidator.IsAccountValid(account))
            {
                return TransactionStatusModel.Fail("Account is expired.");
            }
            
            account.Balance += depositTransactionModel.Amount;
            await _accountsRepository.UpdateAsync(account);

            var transaction = depositTransactionModel.AdaptToEntity(TransactionType.Deposit);
            await _transactionsRepository.AddAsync(transaction);
            
            return TransactionStatusModel.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to perform a deposit for '{SourceAccountId}' account", depositTransactionModel.SourceAccountId);
            throw;
        }
    }

    public async Task<TransactionStatusModel> Withdraw(OneWaySecuredTransactionModel withdrawTransactionModel)
    {
        try
        {
            var account = await _accountsRepository.GetAsync(withdrawTransactionModel.SourceAccountId);
            if (!AccountValidator.IsAccountValid(account, withdrawTransactionModel.SecurityCode))
            {
                return TransactionStatusModel.Fail("Provided account information is not valid. Account is expired or entered " +
                                                   "security code is not correct.");
            }
            
            if (account.Balance < withdrawTransactionModel.Amount)
            {
                return TransactionStatusModel.Fail("Insufficient funds. " +
                                                   "The transaction cannot be completed due to a lack of available funds in the account.");
            }
            
            account.Balance -= withdrawTransactionModel.Amount;
            await _accountsRepository.UpdateAsync(account);

            var transaction = withdrawTransactionModel.AdaptToEntity(TransactionType.Withdrawal);
            await _transactionsRepository.AddAsync(transaction);
            
            return TransactionStatusModel.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to perform a withdrawal from '{SourceAccountId}' account", withdrawTransactionModel.SourceAccountId);
            throw;
        }
    }

    public async Task<TransactionStatusModel> Transfer(TwoWayTransactionModel transferTransactionModel)
    {
        try
        {
            var destinationAccount = await _accountsRepository.GetAsync(transferTransactionModel.DestinationAccountId);
            var sourceAccount = await _accountsRepository.GetAsync(transferTransactionModel.SourceAccountId);
            if (!AccountValidator.IsAccountValid(sourceAccount, transferTransactionModel.SourceAccountSecurityCode))
            {
                return TransactionStatusModel.Fail("Provided account information is not valid. Account is expired or entered " +
                                                   "security code is not correct.");
            }
            if (sourceAccount.Balance < transferTransactionModel.Amount)
            {
                return TransactionStatusModel.Fail("Insufficient funds. " +
                                                   "The transaction cannot be completed due to a lack of available funds in the account.");
            }
            
            sourceAccount.Balance -= transferTransactionModel.Amount;
            destinationAccount.Balance += transferTransactionModel.Amount;
            
            await _accountsRepository.UpdateAsync(sourceAccount);
            await _accountsRepository.UpdateAsync(destinationAccount);

            var transaction = transferTransactionModel.AdaptToEntity(TransactionType.Transfer);
            await _transactionsRepository.AddAsync(transaction);
            
            return TransactionStatusModel.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to perform a transfer from '{SourceAccountId}' account to '{DestinationAccountId}' account", 
                transferTransactionModel.SourceAccountId, transferTransactionModel.DestinationAccountId);
            throw;
        }
    }

    public async Task<decimal> GetBalance(Guid accountId)
    {
        var account = await _accountsRepository.GetAsync(accountId);
        
        return account.Balance;
    }
    
    public async Task<IEnumerable<TransactionResponseModel>> GetAccountTransactionHistory(Guid accountId)
    {
        var transactions = await _transactionsRepository.AccountTransactionHistory(accountId);

        return transactions.Adapt<TransactionResponseModel[]>();
    }
}