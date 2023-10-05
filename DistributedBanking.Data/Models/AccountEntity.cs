namespace DistributedBanking.Data.Models;

public class AccountEntity : BaseEntity
{
    public string Type { get; set; }
    
    public double Balance { get; set; }
    
    public Guid Owner { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime EditedAt { get; set; }
}