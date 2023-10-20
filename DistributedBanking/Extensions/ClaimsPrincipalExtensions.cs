using System.Security.Claims;
using DistributedBanking.Domain.Models;

namespace DistributedBanking.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid Id(this ClaimsPrincipal claimsPrincipal)
    {
        return new Guid(claimsPrincipal.Claims.First(i => i.Type == ClaimConstants.UserIdClaim).Value);
    }
    
    public static string Email(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Claims.First(i => i.Type == ClaimTypes.Email).Value;
    }
}