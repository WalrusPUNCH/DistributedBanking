namespace DistributedBanking.Domain.Models.Transaction;

public class TwoWayTransactionModel
{
    public Guid SourceAccountId { get; set; }
    public required string SourceAccountSecurityCode { get; set; }
    public Guid DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}