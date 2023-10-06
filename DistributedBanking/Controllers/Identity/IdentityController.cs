using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
using DistributedBanking.Data.Models.Identity;
using DistributedBanking.Domain.Models.Identity;
using DistributedBanking.Domain.Services;
using DistributedBanking.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistributedBanking.Controllers.Identity;

[Route("api/identity")]
public class IdentityController : IdentityControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<IdentityController> _logger;

    public IdentityController(
        IIdentityService identityService,
        ILogger<IdentityController> logger) : base(logger)
    {
        _identityService = identityService;
        _logger = logger;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegistrationDto registrationDto)
    { 
        return await RegisterAccount(registrationDto, RoleNames.User);
    }
    
    [HttpPost("register/worker")]
    public async Task<IActionResult> RegisterWorker(RegistrationDto signInModel)
    {
        return await RegisterAccount(signInModel, RoleNames.Worker);
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var loginResult = await _identityService.Login(loginDto.Adapt<LoginModel>());
        if (loginResult.LoginResult.Succeeded)
        {
            return Ok(new JwtTokenDto {Token = loginResult.Token!});
        }
       
        ModelState.AddModelError(nameof(loginDto.Email), "Login Failed: invalid Email or Password");
        throw new ApiException(ModelState.AllErrors());
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _identityService.Logout();
        return Ok();
    }

    private async Task<IActionResult> RegisterAccount(RegistrationDto registrationDto, string role)
    {
        var registrationModel = new RegistrationModel
        {
            Role = role
        };
        
        var userCreationResult = await _identityService.RegisterAccount(registrationDto.Adapt(registrationModel));

        if (userCreationResult.IdentityResult.Succeeded)
        {
            return Created(userCreationResult.User!.Id.ToString(), userCreationResult.User.Adapt<ApplicationUserDto>());
        }
        
        HandleUserManagerFailedResult(userCreationResult.IdentityResult);
        throw new ApiException(ModelState.AllErrors());
    }
}