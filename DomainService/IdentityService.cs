namespace Farsica.Template.DomainService
{
    using Farsica.Template.Data.Dto.Identity;
    using Farsica.Template.Data.Entity.Identity;
    using Farsica.Template.Data.Enumeration;
    using Farsica.Template.Data.Specification;
    using Farsica.Template.Data.Specification.Identity;
    using Farsica.Template.Shared.Service;

    using EntityFramework.Exceptions.Common;

    using Farsica.Framework.Caching;
    using Farsica.Framework.Core;
    using Farsica.Framework.Core.Extensions.Collections.Generic;
    using Farsica.Framework.Core.Extensions.Linq;
    using Farsica.Framework.Data;
    using Farsica.Framework.Data.Enumeration;
    using Farsica.Framework.DataAccess.Specification;
    using Farsica.Framework.DataAccess.Specification.Impl;
    using Farsica.Framework.DataAccess.UnitOfWork;
    using Farsica.Framework.Identity;
    using Farsica.Framework.Mapping;
    using Farsica.Framework.Service;

    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;

    using static Farsica.Framework.Core.Constants;

    using Void = Framework.Data.Void;
    using Error = Framework.Data.Error;

    public class IdentityService(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<IStringLocalizer<IdentityService>> localizer, Lazy<ILogger<IdentityService>> logger
            , Lazy<IApplicationSettingsService> applicationSettingsService, Lazy<UserManager<ApplicationUser>> userManager
            , Lazy<SignInManager<ApplicationUser>> signInManager, Lazy<ICacheProvider> cacheProvider, Lazy<IConfiguration> configuration)
        : LocalizableServiceBase<IdentityService>(unitOfWorkProvider, httpContextAccessor, localizer, logger), IIdentityService, ITokenService
    {
        private const string RolesCacheKey = "Roles";

        public async Task<ResultData<ListDataSource<ApplicationUserDto>>> GetUsersAsync(ListRequestDto<ApplicationUser>? requestDto = null)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var result = await uow.GetRepository<ApplicationUser, int>().GetManyQueryable(requestDto?.Specification).FilterListAsync(requestDto?.PagingDto);
                var users = await result.List.Select(t => new ApplicationUserDto
                {
                    Id = t.Id,
                    Email = t.Email,
                    Enabled = t.Enabled,
                    PhoneNumber = t.PhoneNumber,
                    UserName = t.UserName,
                    RegistrationDate = t.RegistrationDate,
                }).ToListAsync();
                return new(OperationResult.Succeeded) { Data = new ListDataSource<ApplicationUserDto> { List = users, TotalRecordsCount = result.TotalRecordsCount } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<IEnumerable<ApplicationRoleDto>>> GetRolesAsync(ISpecification<ApplicationRoleDto>? specification = null)
        {
            try
            {
                var lst = await cacheProvider.Value.GetAsync<IEnumerable<ApplicationRoleDto>?>(RolesCacheKey, async () =>
                {
                    var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                    return await uow.GetRepository<ApplicationRole, int>().GetManyQueryable().Select(t => new ApplicationRoleDto
                    {
                        Id = t.Id,
                        Name = t.Name!,
                    }).ToListAsync();
                });

                if (lst is not null && specification is not null)
                {
                    lst = lst.Where(specification.IsSatisfiedBy);
                }

                return new(OperationResult.Succeeded) { Data = lst };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<ApplicationUserDto>> GetUserAsync([NotNull] ISpecification<ApplicationUser> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var user = await uow.GetRepository<ApplicationUser, int>().GetManyQueryable(specification).Select(t => new ApplicationUserDto
                {
                    Id = t.Id,
                    Email = t.Email,
                    Enabled = t.Enabled,
                    PhoneNumber = t.PhoneNumber,
                    UserName = t.UserName,
                    RegistrationDate = t.RegistrationDate,
                }).FirstOrDefaultAsync();

                return user is null
                    ? new(OperationResult.NotFound) { Errors = new[] { new Error { Message = Localizer.Value["UserNotFound"] } }, }
                    : new(OperationResult.Succeeded)
                    {
                        Data = new ApplicationUserDto
                        {
                            Id = user.Id,
                            Email = user.Email,
                            Enabled = user.Enabled,
                            PhoneNumber = user.PhoneNumber,
                            UserName = user.UserName,
                            RegistrationDate = user.RegistrationDate,
                        }
                    };
            }
            catch (Exception exc)
            {
                Logger.Value.LogError(exc, nameof(GetUserAsync));
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<ICollection<string>>> GetUserRolesAsync([NotNull] int userId)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var roles = await uow.GetRepository<ApplicationUserRole, int>().GetManyQueryable(t => t.UserId == userId).Select(t => t.Role!.Name!).ToListAsync();
                return new(OperationResult.Succeeded) { Data = roles };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<AuthenticationResponseDto>> AuthenticateAsync([NotNull] AuthenticationRequestDto requestDto)
        {
            try
            {
                var username = requestDto.Username;
                var user = await signInManager.Value.UserManager.FindByNameAsync(username);
                var validationResult = ValidateUser<AuthenticationResponseDto>(user);

                if (validationResult.OperationResult is not OperationResult.Succeeded)
                {
                    return validationResult;
                }

                var signinResult = await signInManager.Value.CheckPasswordSignInAsync(user!, requestDto.Password!, true);
                if (signinResult.Succeeded)
                {
                    var securityStampResult = await userManager.Value.UpdateSecurityStampAsync(user!);
                    if (!securityStampResult.Succeeded)
                    {
                        return new(OperationResult.NotValid)
                        {
                            Errors = securityStampResult.Errors.Select(t => new Error { Message = t.Description, Code = t.Code }),
                        };
                    }
                }

                var message = signinResult.IsLockedOut ? Localizer.Value["UserLockedOut"] : Localizer.Value["WrongUsernameOrPassword"];
                return signinResult.Succeeded
                    ? new(OperationResult.Succeeded) { Data = new AuthenticationResponseDto { User = user!.AdaptData<ApplicationUser, ApplicationUserDto>() } }
                    : new(OperationResult.NotValid)
                    {
                        Errors = new[] { new Error { Message = message } },
                    };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<SignInResponseDto>> SignInAsync([NotNull] SignInRequestDto requestDto)
        {
            try
            {
                var timeZoneId = await GetTimeZoneIdAsync(requestDto.User.Id);
                List<Claim> claims = [
                    new Claim(ClaimTypes.Email, requestDto.User.EmailConfirmed ? requestDto.User.Email! : string.Empty),
                    new Claim(ClaimTypes.MobilePhone, requestDto.User.PhoneNumberConfirmed ? requestDto.User.PhoneNumber! : string.Empty),
                    new Claim(ClaimTypes.System, GenerateDeviceHash(HttpContextAccessor.Value.HttpContext) ?? string.Empty),
                    new Claim(TimeZoneIdClaim, timeZoneId),
                ];
                var user = requestDto.User.AdaptData<ApplicationUserDto, ApplicationUser>();
                var roles = await signInManager.Value.UserManager.GetRolesAsync(user);
                if (roles is not null)
                {
                    for (var i = 0; i < roles.Count; i++)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles[i]));
                    }
                }

                await signInManager.Value.SignInWithClaimsAsync(user, requestDto.RememberMe, claims);

                return new(OperationResult.Succeeded)
                {
                    Data = new SignInResponseDto
                    {
                        Roles = (await GetUserRolesAsync(user.Id)).Data,
                    },
                };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<Void>> SignOutAsync()
        {
            try
            {
                await signInManager.Value.SignOutAsync();

                return new(OperationResult.Succeeded) { Data = new() };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<bool>> CreateUserAsync([NotNull] CreateUserRequestDto requestDto)
        {
            try
            {
                if (string.IsNullOrEmpty(requestDto.Password))
                {
                    return new(OperationResult.NotValid) { Errors = new[] { new Error { Message = Localizer.Value["PasswordIsRequired"], } } };
                }

                var user = new ApplicationUser
                {
                    UserName = requestDto.Username,
                    Email = requestDto.Email,
                    PhoneNumber = requestDto.PhoneNumber,
                    RegistrationDate = DateTime.UtcNow,
                    Enabled = true,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };
                var identityResult = await userManager.Value.CreateAsync(user, requestDto.Password);
                return !identityResult.Succeeded
                    ? new(OperationResult.NotValid) { Data = false, Errors = MapUserManagerErrors(identityResult) }
                    : new(OperationResult.Succeeded) { Data = true };
            }
            catch (UniqueConstraintException)
            {
                return new(OperationResult.NotValid) { Errors = new[] { new Error { Message = Localizer.Value["DuplicateUsername"], } } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<bool>> UpdateUserAsync([NotNull] UpdateUserRequestDto requestDto)
        {
            try
            {
                var user = await userManager.Value.FindByIdAsync(requestDto.Id.ToString());
                if (user is null)
                {
                    return new(OperationResult.NotFound) { Data = false };
                }

                user.Email = requestDto.Email;
                user.PhoneNumber = requestDto.PhoneNumber;
                user.UserName = requestDto.Username;

                var updateUserResult = await userManager.Value.UpdateAsync(user);
                return updateUserResult.Succeeded
                    ? new(OperationResult.Succeeded) { Data = true }
                    : new(OperationResult.NotValid) { Data = false, Errors = MapUserManagerErrors(updateUserResult) };
            }
            catch (UniqueConstraintException)
            {
                return new(OperationResult.NotValid) { Errors = new[] { new Error { Message = Localizer.Value["DuplicateUsername"], } } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Data = false, Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<bool>> ToggleUserAsync([NotNull] ISpecification<ApplicationUser> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var user = await uow.GetRepository<ApplicationUser, int>().GetAsync(specification);
                if (user is null)
                {
                    return new(OperationResult.NotFound)
                    {
                        Data = false,
                        Errors = new[] { new Error { Message = Localizer.Value["UserNotFound"] } },
                    };
                }

                user.Enabled = !user.Enabled;

                _ = uow.GetRepository<ApplicationUser, int>().Update(user);
                _ = await uow.SaveChangesAsync();

                return new(OperationResult.Succeeded) { Data = true };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<bool>> RemoveUserAsync([NotNull] ISpecification<ApplicationUser> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var user = await uow.GetRepository<ApplicationUser, int>().GetAsync(specification);
                if (user is null)
                {
                    return new(OperationResult.NotFound)
                    {
                        Data = false,
                        Errors = new[] { new Error { Message = Localizer.Value["UserNotFound"] } },
                    };
                }

                uow.GetRepository<ApplicationUser, int>().Remove(user);
                _ = await uow.SaveChangesAsync();
                return new(OperationResult.Succeeded) { Data = true };
            }
            catch (ReferenceConstraintException)
            {
                return new(OperationResult.NotValid) { Errors = new[] { new Error { Message = Localizer.Value["UserCantBeRemoved"], } } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<string?>> GetUserTokenAsync([NotNull] GetUserTokenRequestDto requestDto)
        {
            try
            {
                var user = await userManager.Value.FindByIdAsync(requestDto.UserId.ToString());
                if (user is null)
                {
                    return new(OperationResult.NotFound)
                    {
                        Errors = new[] { new Error { Message = Localizer.Value["UserNotFound"] } },
                    };
                }

                var token = await userManager.Value.GetAuthenticationTokenAsync(user, requestDto.TokenProvider, requestDto.Purpose);
                return new(OperationResult.Succeeded) { Data = string.IsNullOrEmpty(token) ? null : $"{requestDto.UserId}{DelimiterAlternate}{token}" };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message } } };
            }
        }

        public async Task<ResultData<GenerateUserTokenResponseDto>> GenerateUserTokenAsync([NotNull] GenerateUserTokenRequestDto requestDto)
        {
            try
            {
                var user = await userManager.Value.FindByIdAsync(requestDto.UserId.ToString());
                if (user is null)
                {
                    return new(OperationResult.NotFound)
                    {
                        Errors = new[] { new Error { Message = Localizer.Value["UserNotFound"] } },
                    };
                }

                var token = await userManager.Value.GenerateUserTokenAsync(user, requestDto.TokenProvider, requestDto.Purpose);
                var setTokenResult = await userManager.Value.SetAuthenticationTokenAsync(user, requestDto.TokenProvider, requestDto.Purpose, token);
                return setTokenResult.Succeeded
                    ? new(OperationResult.Succeeded)
                    {
                        Data = new GenerateUserTokenResponseDto
                        {
                            Token = $"{requestDto.UserId}{DelimiterAlternate}{token}",
                            ExpirationTime = DateTimeOffset.UtcNow.Add(ApiDataProtectorTokenProviderOptions.GetTokenLifespan(configuration.Value)),
                        }
                    }
                    : new(OperationResult.NotValid) { Errors = MapUserManagerErrors(setTokenResult) };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<VerifyTokenResponse?> VerifyTokenAsync([NotNull] VerifyTokenRequest request)
        {
            try
            {
                var user = await userManager.Value.FindByIdAsync(request.UserId!);
                var validationResult = ValidateUser<VerifyTokenResponse>(user);
                if (validationResult.OperationResult is not OperationResult.Succeeded)
                {
                    return null;
                }

                var verifiyTokenResult = await userManager.Value.VerifyUserTokenAsync(user!, request.TokenProvider!, request.Purpose!, request.Token!);
                if (!verifiyTokenResult)
                {
                    return null;
                }

                var timeZoneId = await GetTimeZoneIdAsync(user!.Id);
                List<Claim> claims = [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber!),
                    new Claim(TimeZoneIdClaim, timeZoneId),
                ];

                var roles = await GetUserRolesAsync(user.Id);
                if (roles.Data is not null)
                {
                    foreach (var item in roles.Data)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item!));
                    }
                }

                return new VerifyTokenResponse { Claims = claims };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return null;
            }
        }

        public async Task<ResultData<bool>> RemoveUserTokenAsync([NotNull] RemoveUserTokenRequestDto requestDto)
        {
            try
            {
                var user = await userManager.Value.FindByIdAsync(requestDto.UserId.ToString());
                if (user is null)
                {
                    return new(OperationResult.NotValid) { Errors = new[] { new Error { Message = Localizer.Value["UserNotFound"], } } };
                }

                var removeTokenResult = await userManager.Value.RemoveAuthenticationTokenAsync(user, requestDto.TokenProvider, requestDto.Purpose);
                return removeTokenResult.Succeeded ? new(OperationResult.Succeeded) { Data = true } : new(OperationResult.Failed) { Errors = MapUserManagerErrors(removeTokenResult), };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<bool>> ChangePasswordAsync([NotNull] ChangePasswordRequestDto requestDto)
        {
            try
            {
                var user = await userManager.Value.FindByIdAsync(HttpContextAccessor.Value.HttpContext?.User.UserId<string>()!);
                if (user is null)
                {
                    return new(OperationResult.NotValid) { Errors = new[] { new Error { Message = Localizer.Value["UserNotFound"], } } };
                }
                var changePasswordResult = await userManager.Value.ChangePasswordAsync(user, requestDto.CurrentPassword, requestDto.NewPassword);
                return changePasswordResult.Succeeded
                        ? new(OperationResult.Succeeded) { Data = true }
                        : new(OperationResult.NotValid) { Data = false, Errors = MapUserManagerErrors(changePasswordResult) };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<bool>> ResetPasswordAsync([NotNull] ResetPasswordRequestDto requestDto)
        {
            try
            {
                var user = await userManager.Value.FindByIdAsync(requestDto.UserId.ToString());
                if (user is null)
                {
                    return new(OperationResult.NotValid) { Errors = new[] { new Error { Message = Localizer.Value["UserNotFound"], } } };
                }
                var passwordResetToken = await userManager.Value.GeneratePasswordResetTokenAsync(user);
                var resetPasswordResult = await userManager.Value.ResetPasswordAsync(user, passwordResetToken, requestDto.NewPassword);
                return resetPasswordResult.Succeeded
                        ? new(OperationResult.Succeeded) { Data = true }
                        : new(OperationResult.NotValid) { Data = false, Errors = MapUserManagerErrors(resetPasswordResult) };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task ValidatePrincipalAsync([NotNull] CookieValidatePrincipalContext context)
        {
            var systemClaim = context.Principal?.FindFirstValue(ClaimTypes.System);
            if (string.IsNullOrEmpty(systemClaim))
            {
                await handleUnauthorizedRequestAsync();
                return;
            }

            var hash = GenerateDeviceHash(context.HttpContext);
            if (!systemClaim.Equals(hash, StringComparison.OrdinalIgnoreCase))
            {
                await handleUnauthorizedRequestAsync();
            }

            var userId = context.Principal.UserId<int>();
            var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
            var currentSecurityStamp = await uow.GetRepository<ApplicationUser, int>().GetManyQueryable(t => t.Id == userId).Select(t => t.SecurityStamp).FirstOrDefaultAsync();
            var securityStampClaim = context.Principal?.FindFirstValue(userManager.Value.Options.ClaimsIdentity.SecurityStampClaimType);
            if (currentSecurityStamp != securityStampClaim)
            {
                await handleUnauthorizedRequestAsync();
            }

            async Task handleUnauthorizedRequestAsync()
            {
                context.RejectPrincipal();
                var identityService = context.HttpContext.RequestServices.GetRequiredService<IIdentityService>();
                _ = await identityService.SignOutAsync();
            }
        }

        public async Task<ResultData<UserPermissionsResponseDto>> GetUserPermissionsAsync([NotNull] UserPermissionsRequestDto requestDto)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var data = await uow.GetRepository<ApplicationUser, int>()
                    .GetManyQueryable(new IdEqualsSpecification<ApplicationUser, int>(requestDto.UserId))
                    .Select(t => new
                    {
                        Claims = t.UserClaims!.Where(u => u.ClaimType == PermissionConstants.PermissionPolicy).Select(u => u.ClaimValue).ToList(),
                        Roles = t.UserRoles!.Select(u => u.Role!.Name!).ToList(),
                    }).FirstOrDefaultAsync();

                return data is null
                    ? new(OperationResult.NotFound) { Errors = new[] { new Error { Message = Localizer.Value["UserNotFound"] } }, }
                    : new(OperationResult.Succeeded)
                    {
                        Data = new()
                        {
                            Claims = data.Claims,
                            Roles = data.Roles.ListToFlagsEnum<Role>(),
                        },
                    };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message }, } };
            }
        }

        public async Task<ResultData<Void>> UpdateUserPermissionsAsync([NotNull] UpdateUserPermissionsRequestDto requestDto)
        {
            try
            {
                var user = await userManager.Value.FindByIdAsync(requestDto.UserId.ToString());
                if (user is null)
                {
                    return new(OperationResult.NotValid) { Errors = new[] { new Error { Message = Localizer.Value["UserNotFound"], } } };
                }

                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var userRolesRepository = uow.GetRepository<ApplicationUserRole, int>();
                var userRoles = await userRolesRepository.GetManyQueryable(new UserIdEqualsSpecification<ApplicationUserRole, int>(requestDto.UserId))
                    .Select(t => t.Role!.Name!).ToListAsync();

                if (userRoles.Exists(t => t.Equals(nameof(Role.Admin), StringComparison.OrdinalIgnoreCase)) && requestDto.Roles?.ExistInFlags(Role.Admin) != true)
                {
                    var anotherAdminExists = await userRolesRepository.AnyAsync(t => t.UserId != requestDto.UserId && t.Role!.Name == nameof(Role.Admin));
                    if (!anotherAdminExists)
                    {
                        return new(OperationResult.NotValid) { Errors = new[] { new Error { Message = Localizer.Value["LastAdminCantBeRemoved"] } } };
                    }
                }

                var forceLogout = false;

                var requestRoles = requestDto.Roles?.GetNames();

                IEnumerable<string> newRoles;
                IEnumerable<string> removedRoles;

                if (requestRoles?.Any() != true)
                {
                    newRoles = [];
                    removedRoles = userRoles;
                }
                else
                {
                    newRoles = requestRoles.Except(userRoles);
                    removedRoles = userRoles.Except(requestRoles);
                }

                if (removedRoles.Any())
                {
                    forceLogout = true;
                    _ = await userManager.Value.RemoveFromRolesAsync(user, removedRoles);
                }

                if (newRoles.Any())
                {
                    forceLogout = true;
                    _ = await userManager.Value.AddToRolesAsync(user, newRoles);
                }

                var repository = uow.GetRepository<ApplicationUserClaim, int>();

                var specification = new UserIdEqualsSpecification<ApplicationUserClaim, int>(requestDto.UserId)
                    .And(new ClaimTypeEqualsSpecification(PermissionConstants.PermissionPolicy));
                var claims = await repository.GetManyQueryable(specification)
                    .Select(t => t.ClaimValue).ToListAsync();

                IEnumerable<string?> newPermissions;
                IEnumerable<string?> removedPermissions;

                if (requestDto.Claims?.Any() != true)
                {
                    newPermissions = [];
                    removedPermissions = claims;
                }
                else
                {
                    newPermissions = requestDto.Claims.Except(claims);
                    removedPermissions = claims.Except(requestDto.Claims);
                }

                if (newPermissions.Any())
                {
                    forceLogout = true;
                    foreach (var item in newPermissions)
                    {
                        repository.Add(new ApplicationUserClaim { UserId = requestDto.UserId, ClaimType = PermissionConstants.PermissionPolicy, ClaimValue = item });
                    }
                    _ = await uow.SaveChangesAsync();
                }

                if (removedPermissions.Any())
                {
                    forceLogout = true;
                    _ = await repository.GetManyQueryable(t => t.UserId == requestDto.UserId && t.ClaimType == PermissionConstants.PermissionPolicy && removedPermissions.Contains(t.ClaimValue))
                        .ExecuteDeleteAsync();
                }

                if (forceLogout)
                {
                    _ = await userManager.Value.UpdateSecurityStampAsync(user);
                }

                return new(OperationResult.Succeeded);
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed)
                {
                    Errors = new[] { new Error { Message = exc.Message }, }
                };
            }
        }

        public async Task<ResultData<ProfileSettingsDto>> GetProfileSettingsAsync()
        {
            try
            {
                var userId = HttpContextAccessor.Value.HttpContext?.User.UserId<int>();
                if (!userId.HasValue)
                {
                    return new(OperationResult.Failed)
                    {
                        Errors = new Error[] { new() { Message = Localizer.Value["AuthenticationError"].Value }, },
                    };
                }
                var timeZone = await GetTimeZoneIdAsync(userId.Value);
                return new(OperationResult.Succeeded)
                {
                    Data = new ProfileSettingsDto
                    {
                        TimeZoneId = timeZone,
                    }
                };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message } } };
            }
        }

        public async Task<ResultData<Void>> UpdateProfileSettingsAsync([NotNull] ProfileSettingsDto requestDto)
        {
            try
            {
                var userId = HttpContextAccessor.Value.HttpContext?.User.UserId<int>();
                if (!userId.HasValue)
                {
                    return new(OperationResult.Failed)
                    {
                        Errors = new Error[] { new() { Message = Localizer.Value["AuthenticationError"].Value }, },
                    };
                }

                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var userClaimRepository = uow.GetRepository<ApplicationUserClaim, int>();
                var userClaim = await userClaimRepository.GetManyQueryable(t => t.UserId == userId.Value && t.ClaimType == TimeZoneIdClaim).FirstOrDefaultAsync();

                if (userClaim is null)
                {
                    if (string.IsNullOrEmpty(requestDto.TimeZoneId))
                    {
                        return new(OperationResult.Succeeded);
                    }

                    userClaimRepository.Add(new ApplicationUserClaim
                    {
                        UserId = userId.Value,
                        ClaimType = TimeZoneIdClaim,
                        ClaimValue = requestDto.TimeZoneId,
                    });
                }
                else if (string.IsNullOrEmpty(requestDto.TimeZoneId))
                {
                    userClaimRepository.Remove(userClaim);
                }
                else
                {
                    userClaim.ClaimValue = requestDto.TimeZoneId;
                    _ = userClaimRepository.Update(userClaim);
                }
                _ = await uow.SaveChangesAsync();
                return new(OperationResult.Succeeded);
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message } } };
            }
        }

        private async Task<string> GetTimeZoneIdAsync(int userId)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var timeZoneId = await uow.GetRepository<ApplicationUserClaim, int>().GetManyQueryable(t => t.UserId == userId && t.ClaimType == TimeZoneIdClaim)
                    .Select(t => t.ClaimValue).FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(timeZoneId))
                {
                    return timeZoneId;
                }

                var settings = await applicationSettingsService.Value.GetApplicationSettingsAsync();

                return string.IsNullOrEmpty(settings.Data?.DefaultTimeZoneId)
                    ? IranTimeZoneId
                    : settings.Data.DefaultTimeZoneId;
            }
            catch
            {
                return IranTimeZoneId;
            }
        }

        private static IEnumerable<Error> MapUserManagerErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                yield return new Error { Message = error.Description };
            }
        }

        private static string? GenerateDeviceHash(HttpContext? httpContext)
        {
            var ip = httpContext.GetClientIpAddress();
            var userAgent = httpContext.UserAgent();

            var byteValue = Encoding.UTF8.GetBytes(ip + userAgent);
            var byteHash = SHA256.HashData(byteValue);
            return Convert.ToBase64String(byteHash);
        }

        private ResultData<T> ValidateUser<T>(ApplicationUser? user)
        {
            IEnumerable<Error> errors = [];
            if (user is null)
            {
                errors = [new() { Message = Localizer.Value["UserNotFound"] }];
            }
            else if (!user.Enabled)
            {
                errors = [new() { Message = Localizer.Value["UserNotEnabled"] }];
            }

            return new(user?.Enabled == true ? OperationResult.Succeeded : OperationResult.NotValid)
            {
                Errors = errors,
            };
        }
    }
}
