using DistributedBanking.Data.Models.Constants;

namespace DistributedBanking.Domain.Models.Transaction;

public class TransactionResponseModel
{
    public Guid SourceAccountId { get; set; }
    public Guid? DestinationAccountId { get; set; }
    public required TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Description { get; set; }
}