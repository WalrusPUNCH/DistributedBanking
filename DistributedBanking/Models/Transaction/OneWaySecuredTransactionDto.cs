namespace DistributedBanking.Models.Transaction;

public class OneWaySecuredTransactionDto : OneWayTransactionDto
{
    public required string SecurityCode { get; set; }
}