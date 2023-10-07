using System.ComponentModel.DataAnnotations;

namespace DistributedBanking.Models.Identity;

public class LoginDto
{
    [Required]
    public string Email { get; set; }
 
    [Required]
    public string Password { get; set; }
}