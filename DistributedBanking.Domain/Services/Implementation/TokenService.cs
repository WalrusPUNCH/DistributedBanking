using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DistributedBanking.Data.Models.Identity;
using DistributedBanking.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DistributedBanking.Domain.Services.Implementation;

public class TokenService : ITokenService 
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtOptions _jwtOptions;

    public TokenService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<(string?, string[])> GetTokenAsync(string login, string password)
    {
        var user = await _userManager.FindByEmailAsync(login);
        if (user == null)
        {
            return default;
        }
        
        var jwtSecurityToken = await CreateJwtToken(user);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        
        var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        
        return (token, rolesList.ToArray());
    }
    
    private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(t => new Claim("roles", t)).ToList();
        
        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("uid", user.Id.ToString())
            }
            .Union(userClaims)
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

    public bool ValidateToken(string key, string issuer, string audience, string token)
    {
        throw new NotImplementedException();
    }
}