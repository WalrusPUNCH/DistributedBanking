namespace DistributedBanking.Data.Models.EndUsers;

public class CustomerEntity : EndUserEntityBase
{
    public required Guid[] Accounts { get; set; } = Array.Empty<Guid>();
}