using DistributedBanking.Data.Models.Identity;
using DistributedBanking.Domain.Models;
using DistributedBanking.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace DistributedBanking.Domain.Services;

public interface IIdentityService
{
    Task<IdentityResult> CreateRole(string roleName);

    Task<(IdentityResult IdentityResult, ApplicationUser? User)> RegisterUser(
        EndUserRegistrationModel registrationModel, string role);
    
    Task DeleteUser(string email);

    Task<(SignInResult LoginResult, string? Token)> Login(LoginModel loginModel);

    Task Logout();

    Task<OperationStatusModel> UpdateCustomerPersonalInformation(Guid customerId, CustomerPassportModel customerPassport);
}