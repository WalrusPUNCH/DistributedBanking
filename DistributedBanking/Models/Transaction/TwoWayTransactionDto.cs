using System.ComponentModel.DataAnnotations;

namespace DistributedBanking.Models.Transaction;

public class TwoWayTransactionDto 
{
    [Required]
    public Guid SourceAccountId { get; set; }

    [Required]
    public required string SourceAccountSecurityCode { get; set; }
    
    [Required]
    public Guid DestinationAccountId { get; set; }
    
    [Required, Range(0, double.MaxValue, ErrorMessage = "Value should be greater than 0")]
    public decimal Amount { get; set; }
    
    public string? Description { get; set; }
}