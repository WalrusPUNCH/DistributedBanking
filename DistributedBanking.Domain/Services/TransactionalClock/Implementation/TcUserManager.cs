﻿using DistributedBanking.Data;
using DistributedBanking.Data.Models.Identity.TransactionalClock;
using DistributedBanking.Data.Repositories;
using DistributedBanking.Domain.Models.Identity;
using Mapster;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace DistributedBanking.Domain.Services.TransactionalClock.Implementation;

public class TcUserManager : ITcUserManager
{
    private readonly IUsersTcRepository _usersRepository;
    private readonly IRolesTcRepository _rolesRepository;
    private readonly IPasswordHashingService _passwordService;
    private readonly ILogger<TcUserManager> _logger;

    public TcUserManager(
        IUsersTcRepository usersRepository,
        IRolesTcRepository rolesManager, 
        IPasswordHashingService passwordService,
        ILogger<TcUserManager> logger)
    {
        _usersRepository = usersRepository;
        _rolesRepository = rolesManager;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<IdentityOperationResult> CreateAsync(string endUserId, EndUserRegistrationModel registrationModel, IEnumerable<string>? roles = null)
    {
        try
        {
            var roleNames = roles?.ToList();
            var roleIds = new List<string>();
            if (roleNames != null && roleNames.Any())
            {
                foreach (var roleName in roleNames)
                {
                    var roleId = (await _rolesRepository.GetAsync(r => r.NormalizedName == roleName.NormalizeString())).FirstOrDefault()?.Id;
                    if (roleId != null)
                    {
                        roleIds.Add(roleId.Value.ToString()!);
                    }
                }
            }

            var existingUser = await _usersRepository.GetByEmailAsync(registrationModel.Email);
            if (existingUser != null)
            {
                return IdentityOperationResult.Failed("User with the same email already exists");
            }
            
            var passwordHash = _passwordService.HashPassword(registrationModel.Password, out var salt);
            var user = new ApplicationTcUser
            {
                Email = registrationModel.Email,
                NormalizedEmail = registrationModel.Email.NormalizeString(),
                PhoneNumber = registrationModel.PhoneNumber,
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                CreatedOn = DateTime.UtcNow,
                Roles = roleIds,
                EndUserId = endUserId
            };

            await _usersRepository.AddAsync(user);
            
            return IdentityOperationResult.Success;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occurred while trying to create new user");
            return IdentityOperationResult.Failed();
        }
    }

    public async Task<UserModel?> FindByEmailAsync(string email)
    {
        try
        {
            var user = await _usersRepository.GetByEmailAsync(email);

            return user?.Adapt<UserModel>();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occurred while trying to find user by email");

            return null;
        }
    }

    public async Task<IdentityOperationResult> PasswordSignInAsync(string email, string password)
    {
        try
        {
            var user = await _usersRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return IdentityOperationResult.Failed("User with such email doesn't exist");
            }
        
        
            var successfulSignIn = _passwordService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);

            return successfulSignIn
                ? IdentityOperationResult.Success
                : IdentityOperationResult.Failed("Incorrect email or password");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occurred while trying to sign in user");
            return IdentityOperationResult.Failed();
        }
    }

    public async Task<IEnumerable<string>> GetRolesAsync(ObjectId userId)
    {
        var user = await _usersRepository.GetAsync(userId);
        if (!user.Roles.Any())
        {
            return Array.Empty<string>();
        }

        var roleNames = new List<string>();
        foreach (var roleId in user.Roles)
        {
            roleNames.Add((await _rolesRepository.GetAsync(new ObjectId(roleId))).Name);
        }

        return roleNames;
    }

    public async Task<bool> IsInRoleAsync(ObjectId userId, string roleName)
    {
        var user = await _usersRepository.GetAsync(userId);
        var roleId = (await _rolesRepository.GetAsync(r => r.NormalizedName == roleName.NormalizeString())).FirstOrDefault()?.Id;
        
        return roleId != null && user.Roles.Contains(roleId.Value.ToString());
    }

    public async Task<IdentityOperationResult> DeleteAsync(ObjectId userId)
    {
        try
        {
            await _usersRepository.RemoveAsync(userId);

            return IdentityOperationResult.Success;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occurred while trying to sign in user");
            return IdentityOperationResult.Failed();
        }
    }
}