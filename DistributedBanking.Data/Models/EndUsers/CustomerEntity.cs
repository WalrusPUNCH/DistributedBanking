namespace DistributedBanking.Data.Models.EndUsers;

public class CustomerEntity : EndUserEntityBase
{
    public required List<Guid> Accounts { get; set; } = new ();
    public required CustomerPassport Passport { get; set; }
}