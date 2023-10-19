using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Data.Models.EndUsers;
using DistributedBanking.Data.Models.Identity;
using DistributedBanking.Data.Repositories;
using DistributedBanking.Domain.Models.Identity;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DistributedBanking.Domain.Services.Implementation;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly ICustomersRepository _customersRepository;
    private readonly IWorkersRepository _workersRepository;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService tokenService,
        ICustomersRepository customersRepository,
        IWorkersRepository workersRepository,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _customersRepository = customersRepository;
        _workersRepository = workersRepository;
        _logger = logger;
    }
    
    public async Task<IdentityResult> CreateRole(string roleName)
    {
        var result = await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        if (result.Succeeded)
        {
            _logger.LogInformation("New role '{Role}' has been created", roleName);
        }
        
        return result;
    }

    public async Task<(IdentityResult IdentityResult, ApplicationUser? User)> RegisterAccount(
        EndUserRegistrationModel registrationModel, string role)
    {
        return await RegisterAccountInternal(registrationModel, role);
    }
    
    private async Task<(IdentityResult IdentityResult, ApplicationUser? User)> RegisterAccountInternal(EndUserRegistrationModel registrationModel, string role)
    {
        Guid endUserId;
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
        else
        {
            throw new ArgumentOutOfRangeException(nameof(role), role, "Speicified role is not supported");
        }
        
        var appUser = new ApplicationUser
        {
            EndUserId = endUserId,
            UserName = registrationModel.Email,
            Email = registrationModel.Email,
            PhoneNumber = registrationModel.PhoneNumber,
            SecurityStamp = Guid.NewGuid().ToString()
        };
 
        var userCreationResult = await _userManager.CreateAsync(appUser, registrationModel.Password);
        if (!userCreationResult.Succeeded)
        {
            return (userCreationResult, default);
        }
        
        var roleAssignmentResult = await _userManager.AddToRoleAsync(appUser, role);
        if (!roleAssignmentResult.Succeeded)
        {
            return (roleAssignmentResult, default);
        }
        
        _logger.LogInformation("New user '{Email}' has been registered and assigned a '{Role}' role",
            appUser.Email, role);
            
        return (userCreationResult, appUser);
    }

    public async Task<(SignInResult LoginResult, string? Token)> Login(LoginModel loginModel)
    {
        var appUser = await _userManager.FindByEmailAsync(loginModel.Email);
        if (appUser == null)
        {
            return (SignInResult.Failed, default);
        }
        
        var loginResult = await _signInManager.PasswordSignInAsync(appUser, loginModel.Password, false, false);
        if (!loginResult.Succeeded)
        {
            return (SignInResult.Failed, default);
        }
        
        var token = await _tokenService.GenerateTokenAsync(appUser);
        return (loginResult, token);
    }

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }
}