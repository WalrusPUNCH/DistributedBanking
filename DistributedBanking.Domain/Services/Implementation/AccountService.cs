using DistributedBanking.Data.Models;
using DistributedBanking.Data.Repositories;
using DistributedBanking.Domain.Models.Account;
using Mapster;

namespace DistributedBanking.Domain.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly IAccountsRepository _accountsRepository;
    private readonly ICustomersRepository _customersRepository;

    public AccountService(
        IAccountsRepository accountsRepository, 
        ICustomersRepository customersRepository)
    {
        _accountsRepository = accountsRepository;
        _customersRepository = customersRepository;
    }
    
    public async Task<AccountOwnedResponseModel> CreateAsync(Guid customerId, AccountCreationModel accountCreationModel)
    {
        var account = GenerateNewAccount(customerId, accountCreationModel);
        var accountEntity = account.Adapt<AccountEntity>();
        await _accountsRepository.AddAsync(accountEntity);
        
        var customerEntity = await _customersRepository.GetAsync(customerId);
        customerEntity.Accounts.Add(accountEntity.Id);

        await _customersRepository.UpdateAsync(customerEntity);
        
        return accountEntity.Adapt<AccountOwnedResponseModel>();
    }

    private static AccountModel GenerateNewAccount(Guid customerId, AccountCreationModel accountModel)
    {
        return new AccountModel
        {
            Name = accountModel.Name,
            Type = accountModel.Type,
            Balance = 0,
            ExpirationDate = Generator.GenerateExpirationDate(),
            SecurityCode = Generator.GenerateSecurityCode(),
            Owner = customerId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<AccountOwnedResponseModel> GetAsync(Guid id)
    {
        var account = await _accountsRepository.GetAsync(id);

        return account.Adapt<AccountOwnedResponseModel>();
    }

    public async Task<IEnumerable<AccountResponseModel>> GetCustomerAccountsAsync(Guid customerId)
    {
        var accounts = await _accountsRepository.GetAsync(x => x.Owner == customerId);
        
        return accounts.Adapt<AccountResponseModel[]>();
    }

    public async Task<IEnumerable<AccountOwnedResponseModel>> GetAsync()
    {
        var accounts = await _accountsRepository.GetAsync();
        
        return accounts.Adapt<AccountOwnedResponseModel[]>();
    }

    public async Task UpdateAsync(AccountEntity model)
    {
        await _accountsRepository.UpdateAsync(model);
    }

    public async Task DeleteAsync(Guid id)
    {
        var accountEntity = await _accountsRepository.GetAsync(id);
        if (!accountEntity.Owner.HasValue)
        {
            return;
        }
        
        var customerEntity = await _customersRepository.GetAsync(accountEntity.Owner.Value);
        customerEntity.Accounts.Remove(accountEntity.Id);
        await _customersRepository.UpdateAsync(customerEntity);
        accountEntity.Owner = null;
        await UpdateAsync(accountEntity);
    }
}