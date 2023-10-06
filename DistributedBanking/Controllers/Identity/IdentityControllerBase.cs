using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DistributedBanking.Controllers.Identity;

[ApiController]
//[Authorize(Roles = RoleNames.Worker)]
public class IdentityControllerBase  : ControllerBase
{
    private readonly ILogger<IdentityControllerBase> _logger;

    protected IdentityControllerBase(ILogger<IdentityControllerBase> logger)
    {
        _logger = logger;
    }
    
    protected void HandleUserManagerFailedResult(IdentityResult unsuccessfulResult)
    {
        foreach (var error in unsuccessfulResult.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        
        _logger.LogInformation("Identity action has ended unsuccessfully. Details: {Result}", 
            unsuccessfulResult.ToString());
    }
}