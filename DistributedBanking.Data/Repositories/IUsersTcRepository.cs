using DistributedBanking.Data.Models.Identity.TransactionalClock;
using DistributedBanking.Data.Repositories.Implementation.Default.Base;

namespace DistributedBanking.Data.Repositories;

public interface IUsersTcRepository : IRepositoryBase<ApplicationTcUser>
{
    Task<ApplicationTcUser?> GetByEmailAsync(string email);
}