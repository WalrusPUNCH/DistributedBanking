namespace DistributedBanking.Domain.Services.TransactionalClock;

public interface IPasswordHashingService
{
    string HashPassword(string password, out string salt);
    bool VerifyPassword(string password, string hash, string salt);
}