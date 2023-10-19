namespace DistributedBanking.Models.Identity;

public class ApplicationUserDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid[] Roles { get; set; } = Array.Empty<Guid>();
}