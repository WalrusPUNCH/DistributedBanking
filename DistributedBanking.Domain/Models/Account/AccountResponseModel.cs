using DistributedBanking.Data.Models.Constants;

namespace DistributedBanking.Domain.Models.Account;

public class AccountResponseModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public AccountType Type { get; set; }
    public double Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}