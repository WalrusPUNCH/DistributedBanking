using DistributedBanking.Data.Models.Identity.TransactionalClock;
using DistributedBanking.Domain.Models.Identity;

namespace DistributedBanking.Domain.Services.TransactionalClock;

public interface ITcRolesManager
{
    Task<IdentityOperationResult> CreateAsync(ApplicationTcRole role);
    Task<bool> RoleExists(string roleName);
}