using DistributedBanking.Data.Models.Identity.Default;

namespace DistributedBanking.Domain.Services.Default;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}