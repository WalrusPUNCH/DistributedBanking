namespace DistributedBanking.Domain.Services;

public interface ITokenService
{
    Task<(string?, string[] roles)> GetTokenAsync(string login, string password);
    bool ValidateToken(string key, string issuer, string audience, string token);
}