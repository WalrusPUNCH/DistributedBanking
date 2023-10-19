using System.ComponentModel.DataAnnotations;

namespace DistributedBanking.Models.Identity;

public class LoginDto
{
    [Required]
    public required string Email { get; set; }
 
    [Required]
    public required string Password { get; set; }
}