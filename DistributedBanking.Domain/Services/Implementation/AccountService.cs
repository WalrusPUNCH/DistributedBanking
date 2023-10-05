using DistributedBanking.Data.Models;
using DistributedBanking.Data.Repositories;

namespace DistributedBanking.Domain.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly IAccountsRepository _accountsRepository;

    public AccountService(IAccountsRepository accountsRepository)
    {
        _accountsRepository = accountsRepository;
    }
    
    public async Task CreateAsync(AccountEntity model)
    {
        model.CreatedAt = DateTime.UtcNow;
        
        await _accountsRepository.CreateAsync(model);
    }

    public async Task<AccountEntity> GetAsync(Guid id)
    {
        return await _accountsRepository.GetAsync(id);
    }

    public async Task<IEnumerable<AccountEntity>> GetAsync()
    {
        return await _accountsRepository.GetAsync();
    }

    public async Task UpdateAsync(AccountEntity model)
    {
        model.EditedAt = DateTime.UtcNow;

        await _accountsRepository.UpdateAsync(model);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _accountsRepository.RemoveAsync(id);
    }
}