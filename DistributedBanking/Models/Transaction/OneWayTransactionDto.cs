﻿using System.ComponentModel.DataAnnotations;

namespace DistributedBanking.Models.Transaction;

public class OneWayTransactionDto
{
    [Required]
    public Guid SourceAccountId { get; set; }
    
    [Required, Range(0, double.MaxValue, ErrorMessage = "Value should be greater than 0")]
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}