using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Data.Models.EndUsers;
using DistributedBanking.Data.Models.Identity.Default;
using DistributedBanking.Data.Repositories;
using DistributedBanking.Domain.Extensions;
using DistributedBanking.Domain.Models;
using DistributedBanking.Domain.Models.Identity;
using DistributedBanking.Domain.Services.Base;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace DistributedBanking.Domain.Services.Default.Implementation;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly ICustomersRepository _customersRepository;
    private readonly IWorkersRepository _workersRepository;
    private readonly IAccountService _accountService;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService tokenService,
        ICustomersRepository customersRepository,
        IWorkersRepository workersRepository,
        IAccountService accountService,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _customersRepository = customersRepository;
        _workersRepository = workersRepository;
        _accountService = accountService;
        _logger = logger;
    }
    
    public async Task<IdentityOperationResult> CreateRole(string roleName)
    {
        var result = await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        if (result.Succeeded)
        {
            _logger.LogInformation("New role '{Role}' has been created", roleName);
        }
        
        return result.ToDomainIdentityResult();
    }

    public async Task<IdentityOperationResult> RegisterUser(
        EndUserRegistrationModel registrationModel, string role)
    {
        return await RegisterUserInternal(registrationModel, role);
    }
    
    private async Task<IdentityOperationResult> RegisterUserInternal(EndUserRegistrationModel registrationModel, string role)
    {
        ObjectId endUserId;
        if (string.Equals(role, RoleNames.Customer, StringComparison.InvariantCultureIgnoreCase))
        {
            var customerEntity = registrationModel.Adapt<CustomerEntity>();
            await _customersRepository.AddAsync(customerEntity);

            endUserId = customerEntity.Id;
        }
        else if (string.Equals(role, RoleNames.Worker, StringComparison.InvariantCultureIgnoreCase))
        {
            var workerEntity = registrationModel.Adapt<WorkerEntity>();
            await _workersRepository.AddAsync(workerEntity);
            
            endUserId = workerEntity.Id;
        }
        else if (string.Equals(role, RoleNames.Administrator, StringComparison.InvariantCultureIgnoreCase))
        {
            var workerEntity = registrationModel.Adapt<WorkerEntity>();
            await _workersRepository.AddAsync(workerEntity);
            
            endUserId = workerEntity.Id;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(role), role, "Specified role is not supported");
        }
        
        var appUser = new ApplicationUser
        {
            EndUserId = endUserId.ToString(),
            UserName = registrationModel.Email,
            Email = registrationModel.Email,
            PhoneNumber = registrationModel.PhoneNumber,
            SecurityStamp = Guid.NewGuid().ToString()
        };
 
        var userCreationResult = await _userManager.CreateAsync(appUser, registrationModel.Password);
        if (!userCreationResult.Succeeded)
        {
            return userCreationResult.ToDomainIdentityResult();
        }
        
        var roleAssignmentResult = await _userManager.AddToRoleAsync(appUser, role);
        if (!roleAssignmentResult.Succeeded)
        {
            return roleAssignmentResult.ToDomainIdentityResult();
        }
        
        _logger.LogInformation("New user '{Email}' has been registered and assigned a '{Role}' role",
            appUser.Email, role);
            
        return userCreationResult.ToDomainIdentityResult();
    }

    public async Task<(IdentityOperationResult LoginResult, string? Token)> Login(LoginModel loginModel)
    {
        var appUser = await _userManager.FindByEmailAsync(loginModel.Email);
        if (appUser == null)
        {
            return (IdentityOperationResult.Failed("User with such email doesn't exist"), default);
        }
        
        var loginResult = await _signInManager.PasswordSignInAsync(appUser, loginModel.Password, false, false);
        if (!loginResult.Succeeded)
        {
            return (IdentityOperationResult.Failed("Incorrect email or password"), default);
        }
        
        var token = await _tokenService.GenerateTokenAsync(appUser);
        return (IdentityOperationResult.Success, token);
    }

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task DeleteUser(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser != null)
        {
            if (await _userManager.IsInRoleAsync(appUser, RoleNames.Customer))
            {
                var customer = await _customersRepository.GetAsync(new ObjectId(appUser.EndUserId));
                foreach (var customerAccountId in customer.Accounts)
                {
                    await _accountService.DeleteAsync(customerAccountId);
                }
                
                await _customersRepository.RemoveAsync(new ObjectId(appUser.EndUserId));
            }
            else if (await _userManager.IsInRoleAsync(appUser, RoleNames.Worker))
            {
                await _workersRepository.RemoveAsync(new ObjectId(appUser.EndUserId));
            }
            
            await _userManager.DeleteAsync(appUser);
        }
    }

    public async Task<OperationStatusModel> UpdateCustomerPersonalInformation(string customerId, CustomerPassportModel customerPassport)
    {
        try
        {
            var customer = await _customersRepository.GetAsync(new ObjectId(customerId));
            customer.Passport = customerPassport.Adapt<CustomerPassport>();

            await _customersRepository.UpdateAsync(customer);
            
            return OperationStatusModel.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to update personal information. Try again later");
            throw;
        }
    }
}