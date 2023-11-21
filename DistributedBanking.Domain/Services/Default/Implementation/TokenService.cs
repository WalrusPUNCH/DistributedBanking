using System.IdentityModel.Tokens.Jwt;
using DistributedBanking.Data.Models.Identity.Default;
using DistributedBanking.Domain.Options;
using DistributedBanking.Domain.Services.Base.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DistributedBanking.Domain.Services.Default.Implementation;

public class TokenService : TokenServiceBase, ITokenService 
{
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtOptions> jwtOptions) : base(jwtOptions)
    {
        _userManager = userManager;
    }

    public async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var jwtSecurityToken = CreateJwtToken(roles, user.Email, user.EndUserId);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        
        return token;
    }
}