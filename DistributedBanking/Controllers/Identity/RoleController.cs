using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
}