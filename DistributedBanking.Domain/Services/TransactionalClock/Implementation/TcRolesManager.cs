using DistributedBanking.Data;
using DistributedBanking.Data.Models.Identity.TransactionalClock;
using DistributedBanking.Data.Repositories;
using DistributedBanking.Domain.Models.Identity;
using Microsoft.Extensions.Logging;

namespace DistributedBanking.Domain.Services.TransactionalClock.Implementation;

public class TcRolesManager : ITcRolesManager
{
    private readonly IRolesTcRepository _rolesRepository;
    private readonly ILogger<TcRolesManager> _logger;

    public TcRolesManager(
        IRolesTcRepository rolesRepository,
        ILogger<TcRolesManager> logger)
    {
        _rolesRepository = rolesRepository;
        _logger = logger;
    }
    
    public async Task<IdentityOperationResult> CreateAsync(ApplicationTcRole role)
    {
        try
        {
            await _rolesRepository.AddAsync(role);
            return IdentityOperationResult.Success;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception occurred while trying to create new role");
            
            return IdentityOperationResult.Failed();
        }
    }

    public async Task<bool> RoleExists(string roleName)
    {
        return (await _rolesRepository.GetAsync(r => r.NormalizedName == roleName.NormalizeString())).Any();
    }
}