using DistributedBanking.Data.Models.Identity;
using DistributedBanking.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace DistributedBanking.Domain.Services;

public interface IIdentityService
{
    Task<IdentityResult> CreateRole(string roleName);

    Task<(IdentityResult IdentityResult, ApplicationUser? User)> RegisterAccount(RegistrationModel registrationModel);

    Task<(SignInResult LoginResult, string? Token)> Login(LoginModel loginModel);

    Task Logout();
}