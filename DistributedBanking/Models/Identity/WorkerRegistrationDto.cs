using System.ComponentModel.DataAnnotations;

namespace DistributedBanking.Models.Identity;

public class WorkerRegistrationDto : EndUserRegistrationDto
{
    [Required]
    public string Position { get; set; }
    
    [Required]
    public AddressDto Address { get; set; }
}