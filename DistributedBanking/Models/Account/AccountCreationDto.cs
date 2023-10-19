using System.ComponentModel.DataAnnotations;
using DistributedBanking.Data.Models.Constants;

namespace DistributedBanking.Models.Account;

public class AccountCreationDto
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public AccountType Type { get; set; }
}