namespace DistributedBanking.Domain.Models.Transaction;

public class OneWaySecuredTransactionModel : OneWayTransactionModel
{
    public string SecurityCode { get; set; }
}