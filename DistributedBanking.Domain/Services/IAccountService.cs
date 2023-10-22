using DistributedBanking.Data.Models;
using DistributedBanking.Domain.Models.Account;

namespace DistributedBanking.Domain.Services;

public interface IAccountService
{
    Task<AccountOwnedResponseModel> CreateAsync(Guid customerId, AccountCreationModel accountModel);
    Task<AccountOwnedResponseModel> GetAsync(Guid id);
    Task<IEnumerable<AccountResponseModel>> GetCustomerAccountsAsync(Guid customerId);
    Task<bool> BelongsTo(Guid accountId, Guid customerId);
    Task<IEnumerable<AccountOwnedResponseModel>> GetAsync();
    Task UpdateAsync(AccountEntity model);
    Task DeleteAsync(Guid id);
}