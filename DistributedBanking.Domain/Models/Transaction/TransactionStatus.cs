namespace DistributedBanking.Domain.Models.Transaction;

public class TransactionStatusModel
{
    public bool EndedSuccessfully { get; set; }
    public string? Message { get; set; }

    public static TransactionStatusModel Success() => new()
    {
        EndedSuccessfully = true
    };
    
    public static TransactionStatusModel Fail(string? message) => new()
    {
        EndedSuccessfully = false, 
        Message = message
    };
}