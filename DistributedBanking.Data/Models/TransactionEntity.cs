using DistributedBanking.Data.Models.Constants;

namespace DistributedBanking.Data.Models;

public class TransactionEntity : BaseEntity
{
    public Guid SourceAccountId { get; set; }
    public Guid? DestinationAccountId { get; set; }
    public required TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime DateTime { get; set; } //todo rename to Timestamp
    public string? Description { get; set; }
}