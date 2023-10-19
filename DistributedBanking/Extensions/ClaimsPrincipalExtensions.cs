using System.Security.Claims;
using DistributedBanking.Domain.Models;

namespace DistributedBanking.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetId(this ClaimsPrincipal claimsPrincipal)
    {
        return new Guid(claimsPrincipal.Claims.First(i => i.Type == ClaimConstants.UserIdClaim).Value);
    }
}