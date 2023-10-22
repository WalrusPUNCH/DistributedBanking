namespace DistributedBanking.Domain.Models.Transaction;

public class OneWayTransactionModel
{
    public Guid SourceAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}