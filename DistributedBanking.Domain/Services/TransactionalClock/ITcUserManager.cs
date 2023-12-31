﻿using DistributedBanking.Domain.Models.Identity;
using MongoDB.Bson;

namespace DistributedBanking.Domain.Services.TransactionalClock;

public interface ITcUserManager
{
    Task<IdentityOperationResult> CreateAsync(string endUserId, EndUserRegistrationModel registrationModel, IEnumerable<string>? roles = null);
    Task<UserModel?> FindByEmailAsync(string email);
    Task<IdentityOperationResult> PasswordSignInAsync(string email, string password);
    Task<IEnumerable<string>> GetRolesAsync(ObjectId userId);
    Task<bool> IsInRoleAsync(ObjectId userId, string roleName);
    Task<IdentityOperationResult> DeleteAsync(ObjectId userId);
}
