namespace DistributedBanking.Domain.Models.Identity;

public class WorkerRegistrationModel : EndUserRegistrationModel
{
    public string Position { get; set; }
    
    public AddressModel Address { get; set; }
}