using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
using DistributedBanking.Data.Models.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using DistributedBanking.Domain.Services.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace DistributedBanking.Controllers.Identity;

[Route("api/identity/role")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleNames.Administrator)]
public class RoleController : IdentityControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<RoleController> _logger;

    public RoleController(
        IIdentityService identityService,
        ILogger<RoleController> logger) : base(logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateRole([Required] string roleName)
    {
        var result = await _identityService.CreateRole(roleName);
        if (result.Succeeded)
        {
            return Ok();
        }

        HandleUserManagerFailedResult(result);
        throw new ApiException(ModelState.AllErrors());
    }
    
    [HttpGet("initialize")]
    public async Task<IActionResult> Initialize()
    {
        var roles = new List<string> { RoleNames.Administrator, RoleNames.Worker, RoleNames.Customer};

        foreach (var roleName in roles)
        {
            var result = await _identityService.CreateRole(roleName);
            if (result.Succeeded)
            {
                continue;
            }

            HandleUserManagerFailedResult(result);
            throw new ApiException(ModelState.AllErrors());
        }

        return Ok();
    }
}