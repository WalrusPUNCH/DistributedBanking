using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Domain.Models.Identity;
using DistributedBanking.Domain.Services;
using DistributedBanking.Extensions;
using DistributedBanking.Models.Identity;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    
    [HttpPost("register/customer")]
    [ProducesResponseType(typeof(ApplicationUserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterCustomer(EndUserRegistrationDto registrationDto)
    { 
        return await RegisterUser(registrationDto.Adapt<EndUserRegistrationModel>(), RoleNames.Customer);
    }
    
    [HttpPost("register/worker")]
    [ProducesResponseType(typeof(ApplicationUserDto), StatusCodes.Status200OK)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleNames.Administrator)]
    public async Task<IActionResult> RegisterWorker(WorkerRegistrationDto registrationDto)
    {
        return await RegisterUser(registrationDto.Adapt<WorkerRegistrationModel>(), RoleNames.Worker);
    }
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(JwtTokenDto), StatusCodes.Status200OK)]
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
    
    [HttpGet("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        await _identityService.Logout();
        return Ok();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleNames.Customer)]
    public async Task<IActionResult> Delete()
    {
        var userEmail = User.Email();
        
        await _identityService.DeleteUser(userEmail);
        return Ok();
    }

    private async Task<IActionResult> RegisterUser(EndUserRegistrationModel registrationModel, string role)
    {
        var userCreationResult = await _identityService.RegisterUser(registrationModel, role);

        if (userCreationResult.IdentityResult.Succeeded)
        {
            return Created(userCreationResult.User!.Id.ToString(), userCreationResult.User.Adapt<ApplicationUserDto>());
        }
        
        HandleUserManagerFailedResult(userCreationResult.IdentityResult);
        throw new ApiException(ModelState.AllErrors());
    }
}