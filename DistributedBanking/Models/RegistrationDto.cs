using System.ComponentModel.DataAnnotations;

namespace DistributedBanking.Models;

public class RegistrationDto
{
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    public DateTime BirthDate { get; set; }

    [Required, Phone]
    public string PhoneNumber { get; set; }
 
    [Required, EmailAddress(ErrorMessage = "Invalid Email")]
    public string Email { get; set; }
 
    [Required]
    public string Password { get; set; }
}