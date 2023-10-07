using System.ComponentModel.DataAnnotations;

namespace DistributedBanking.Models.Identity;

public class AddressDto
{
    [Required]
    public string Country { get; set; }
    
    [Required]
    public string Region { get; set; }
    
    [Required]
    public string City { get; set; }
    
    [Required]
    public string Street { get; set; }
    
    [Required]
    public string Building { get; set; }
    
    [Required]
    public string PostalCode { get; set; }
}