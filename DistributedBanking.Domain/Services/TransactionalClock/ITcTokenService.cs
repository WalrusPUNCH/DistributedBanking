using DistributedBanking.Domain.Models.Identity;

namespace DistributedBanking.Domain.Services.TransactionalClock;

public interface ITcTokenService
{
    Task<string> GenerateTokenAsync(UserModel user);
}