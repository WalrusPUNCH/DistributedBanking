using DistributedBanking.Domain.Models;
using DistributedBanking.Domain.Models.Identity;

namespace DistributedBanking.Domain.Services.Base;

public interface IIdentityService
{
    Task<IdentityOperationResult> CreateRole(string roleName);

    Task<IdentityOperationResult> RegisterUser(
        EndUserRegistrationModel registrationModel, string role);
    
    Task DeleteUser(string email);

    Task<(IdentityOperationResult LoginResult, string? Token)> Login(LoginModel loginModel);

    Task Logout();

    Task<OperationStatusModel> UpdateCustomerPersonalInformation(string customerId, CustomerPassportModel customerPassport);
}