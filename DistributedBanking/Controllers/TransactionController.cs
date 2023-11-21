using DistributedBanking.Data.Models.Constants;
using DistributedBanking.Domain.Models.Transaction;
using DistributedBanking.Domain.Services.Base;
using DistributedBanking.Filters;
using DistributedBanking.Models.Transaction;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistributedBanking.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleNames.Customer)]
[TypeFilter(typeof(UserAccountCheckingActionFilterAttribute))]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Route("api/transaction")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionController> _logger;
    
    public TransactionController(
        ITransactionService transactionService,
        ILogger<TransactionController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    [HttpPost("deposit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deposit(OneWayTransactionDto depositTransactionDto)
    {
        var depositStatus = await _transactionService.Deposit(depositTransactionDto.Adapt<OneWayTransactionModel>());
        
        return depositStatus.EndedSuccessfully 
            ? Ok() 
            : BadRequest(depositStatus.Message);
    }
    
    [HttpPost("withdraw")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Withdraw(OneWaySecuredTransactionDto withdrawalTransactionDto)
    {
        var withdrawalStatus = await _transactionService.Withdraw(withdrawalTransactionDto.Adapt<OneWaySecuredTransactionModel>());
        
        return withdrawalStatus.EndedSuccessfully 
            ? Ok() 
            : BadRequest(withdrawalStatus.Message);
    }
    
    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Transfer(TwoWayTransactionDto transferTransactionDto)
    {
        var transferStatus = await _transactionService.Transfer(transferTransactionDto.Adapt<TwoWayTransactionModel>());
        
        return transferStatus.EndedSuccessfully 
            ? Ok() 
            : BadRequest(transferStatus.Message);
    }
    
    [HttpPost("account_history/{accountId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AccountHistory(string accountId)
    {
        var transferStatus = await _transactionService.GetAccountTransactionHistory(accountId);
        
        return Ok(transferStatus);
    }
}