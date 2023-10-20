using DistributedBanking.Data.Models.Constants;

namespace DistributedBanking.Data.Models;

public class AccountEntity : BaseEntity
{
    public required string Name { get; set; }
    public AccountType Type { get; set; }
    public double Balance { get; set; }
    public DateTime ExpirationDate { get; set; }
    public required string SecurityCode { get; set; }
    public Guid? Owner { get; set; }
    public DateTime CreatedAt { get; set; }
}