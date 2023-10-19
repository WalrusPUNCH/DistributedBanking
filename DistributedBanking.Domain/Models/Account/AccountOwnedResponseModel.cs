namespace DistributedBanking.Domain.Models.Account;

public class AccountOwnedResponseModel : AccountResponseModel
{
    public Guid Owner { get; set; }
}