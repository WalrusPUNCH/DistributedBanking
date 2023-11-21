using System.IdentityModel.Tokens.Jwt;
using DistributedBanking.Domain.Models.Identity;
using DistributedBanking.Domain.Options;
using DistributedBanking.Domain.Services.Base.Implementation;
using Microsoft.Extensions.Options;

namespace DistributedBanking.Domain.Services.TransactionalClock.Implementation;

public class TcTokenService : TokenServiceBase, ITcTokenService
{
    private readonly ITcUserManager _tcUserManager;

    public TcTokenService(
        ITcUserManager tcUserManager,
        IOptions<JwtOptions> jwtOptions) : base(jwtOptions)
    {
        _tcUserManager = tcUserManager;
    }

    public async Task<string> GenerateTokenAsync(UserModel user)
    {
        var roles = await _tcUserManager.GetRolesAsync(user.Id);
        var jwtSecurityToken = CreateJwtToken(roles, user.Email, user.EndUserId.ToString());
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        
        return token;
    }
}