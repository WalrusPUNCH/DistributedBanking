using System.ComponentModel.DataAnnotations;
using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
using DistributedBanking.Data.Models.Identity;
using DistributedBanking.Domain.Services;
using DistributedBanking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DistributedBanking.Controllers;

[ApiController]
[Route("api/identity")]
//[Authorize(Roles = RoleNames.Worker)]
public class IdentityController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<IdentityController> _logger;

    public IdentityController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ILogger<IdentityController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _logger = logger;
    }
    
    [HttpPost("role")]
    public async Task<IActionResult> CreateRole([Required] string name)
    {
        var result = await _roleManager.CreateAsync(new ApplicationRole { Name = name });
        if (result.Succeeded) 
            return Ok();
        else
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
            
            throw new ApiException(ModelState.AllErrors());
        }
        
    }
    
    [HttpPost("sign_in")]
    public async Task<IActionResult> Create(SignInDto signInModel)
    { 
        return await CreateAccount(signInModel, RoleNames.User);
    }
    
    [HttpPost("sign_in/worker")]
    public async Task<IActionResult> CreateWorker(SignInDto signInModel)
    {
        return await CreateAccount(signInModel, RoleNames.Worker);
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [Required] string email, //todo move it to the separate class
        [Required] string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser != null)
        {
            var result = await _signInManager.PasswordSignInAsync(appUser, password, false, false);
            if (result.Succeeded)
            {
                var token = await _tokenService.GetTokenAsync(email, password);
                return Ok(new {Token = token.Item1, Roles = token.Item2});
            }
        }
            
        ModelState.AddModelError(nameof(email), "Login Failed: Invalid Email or Password");
        throw new ApiException(ModelState.AllErrors());
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    private async Task<IActionResult> CreateAccount(SignInDto signIn, string role)
    {
        var appUser = new ApplicationUser
        {
            UserName = signIn.Name,
            Email = signIn.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };
 
        var creationResult = await _userManager.CreateAsync(appUser, signIn.Password);
        var roleAssignmentResult = await _userManager.AddToRoleAsync(appUser, role);
        
        if (creationResult.Succeeded && roleAssignmentResult.Succeeded)
        {
            return Created(appUser.Id.ToString(), appUser); //todo don't return sensitive info
        }
        else
        {
            HandleUserManagerFailedResult(creationResult);
            HandleUserManagerFailedResult(roleAssignmentResult);
            
            throw new ApiException(ModelState.AllErrors());
        }
    }
    
    private void HandleUserManagerFailedResult(IdentityResult unsuccessfulResult)
    {
        foreach (var error in unsuccessfulResult.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
    }
}