namespace DistributedBanking.Data.Models;

public class CustomerEntity : BaseEntity
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required DateTime BirthDate { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Email { get; init; }
    public required Guid[] Accounts { get; set; } = Array.Empty<Guid>();
}