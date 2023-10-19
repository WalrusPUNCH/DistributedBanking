using System.ComponentModel.DataAnnotations;
using DistributedBanking.Data.Models.Constants;

namespace DistributedBanking.Models.Account;

public class AccountCreationDto
{
    [Required]
    public required string Name { get; set; }
    
    [Required]
    public required AccountType Type { get; set; }
}