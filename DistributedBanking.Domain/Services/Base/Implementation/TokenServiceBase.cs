using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DistributedBanking.Domain.Models;
using DistributedBanking.Domain.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DistributedBanking.Domain.Services.Base.Implementation;

public abstract class TokenServiceBase
{
    private readonly JwtOptions _jwtOptions;

    protected TokenServiceBase(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }
    
    protected JwtSecurityToken CreateJwtToken(
        IEnumerable<string> roles,
        string email,
        string endUserId)
    {
        var roleClaims = roles.Select(t => new Claim("roles", t)).ToList();
        
        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimConstants.UserIdClaim, endUserId)
            }
            .Union(roleClaims);
        
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: signingCredentials);
        
        return jwtSecurityToken;
    }
}