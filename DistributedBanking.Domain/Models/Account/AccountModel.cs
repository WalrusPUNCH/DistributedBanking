using DistributedBanking.Data.Models.Constants;

namespace DistributedBanking.Domain.Models.Account;

public class AccountModel
{
    public string Name { get; set; }
    public AccountType Type { get; set; }
    public double Balance { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string SecurityCode { get; set; }
    public Guid Owner { get; set; }
    public DateTime CreatedAt { get; set; }
}