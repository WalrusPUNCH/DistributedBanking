namespace DistributedBanking.Domain.Models.Identity;

public class AddressModel
{
    public string Country { get; set; }
    public string Region { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string Building { get; set; }
    public string PostalCode { get; set; }
}