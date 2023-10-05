﻿using DistributedBanking.Data.Models;
using DistributedBanking.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace DistributedBanking.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] AccountEntity model)
    {
        try
        {
            await _accountService.CreateAsync(model);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAccount([FromQuery] Guid id)
    {
        try
        {
            var item = await _accountService.GetAsync(id);
            return Ok(item);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        try
        {
            var items = await _accountService.GetAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}