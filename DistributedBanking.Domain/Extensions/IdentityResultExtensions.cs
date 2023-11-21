using DistributedBanking.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace DistributedBanking.Domain.Extensions;

public static class IdentityResultExtensions
{
    public static IdentityOperationResult ToDomainIdentityResult(this IdentityResult identityResult)
    {
        return identityResult.Succeeded 
            ? IdentityOperationResult.Success 
            : IdentityOperationResult.Failed(identityResult.Errors.Select(e => e.Description).ToArray());
    }
}