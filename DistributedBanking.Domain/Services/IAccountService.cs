using DistributedBanking.Data.Models;

namespace DistributedBanking.Domain.Services;

public interface IAccountService
{
    Task CreateAsync(AccountEntity model);
    Task<AccountEntity> GetAsync(Guid id);
    Task<IEnumerable<AccountEntity>> GetAsync();
    Task UpdateAsync(AccountEntity model);
    Task DeleteAsync(Guid id);
}