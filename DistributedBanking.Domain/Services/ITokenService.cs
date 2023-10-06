using DistributedBanking.Data.Models.Identity;

namespace DistributedBanking.Domain.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}