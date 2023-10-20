using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Domain.Models.Account;
using DistributedBanking.Domain.Services;
using DistributedBanking.Extensions;
using DistributedBanking.Models.Account;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistributedBanking.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleNames.Customer)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AccountOwnedResponseModel), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAccount(AccountCreationDto accountDto)
    {
        var userId = User.Id();
        var createdAccount = await _accountService.CreateAsync(userId, accountDto.Adapt<AccountCreationModel>());
            
        return Created(createdAccount.Id.ToString(), createdAccount);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountOwnedResponseModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccounts()
    {
        var items = await _accountService.GetAsync();
        
        return Ok(items);
    }
    
    [HttpGet("owned/{ownerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<AccountResponseModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomerAccounts(Guid ownerId)
    {
        var items = await _accountService.GetCustomerAccountsAsync(ownerId);
        
        return Ok(items);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccountOwnedResponseModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccount(Guid id)
    {
        var item = await _accountService.GetAsync(id);
        
        return Ok(item);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        await _accountService.DeleteAsync(id);
        
        return Ok();
    }
}