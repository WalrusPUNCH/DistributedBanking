using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DistributedBanking.Controllers.Identity;

[ApiController]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
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