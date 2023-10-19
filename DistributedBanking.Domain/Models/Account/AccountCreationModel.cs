using DistributedBanking.Data.Models.Constants;

namespace DistributedBanking.Domain.Models.Account;

public class AccountCreationModel
{
    public string Name { get; set; }
    public AccountType Type { get; set; }
}