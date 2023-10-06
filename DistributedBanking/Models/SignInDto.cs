using System.ComponentModel.DataAnnotations;

namespace DistributedBanking.Models;

public class SignInDto
{
    [Required]
    public string Name { get; set; }
 
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string Email { get; set; }
 
    [Required]
    public string Password { get; set; }
}