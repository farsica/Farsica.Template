namespace Farsica.Template.UI.Web.Api
{
    using Farsica.Template.Data.Dto.Identity;
    using Farsica.Template.Data.Enumeration;
    using Farsica.Template.Data.ViewModel.Identity;
    using Farsica.Template.Shared.Service;

    using Asp.Versioning;

    using Farsica.Framework.Core;
    using Farsica.Framework.Data;
    using Farsica.Framework.Data.Enumeration;
    using Farsica.Framework.DataAnnotation;
    using Farsica.Framework.Identity;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Diagnostics.CodeAnalysis;

    using static Farsica.Framework.Core.Constants;

    using Void = Framework.Data.Void;

    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    [Display(Name = "احراز هویت")]
    public class IdentitiesController(Lazy<ILogger<IdentitiesController>> logger, Lazy<IIdentityService> identityService) : ApiControllerBase<IdentitiesController>(logger)
    {
        [HttpPost("[action]"), Produces(typeof(ApiResponse<AuthenticationResponseViewModel>))]
        [AllowAnonymous]
        [Display(Name = "ورود کاربر")]
        public async Task<IActionResult> Login([NotNull] AuthenticationRequestViewModel request)
        {
            try
            {
                var authenticateResult = await identityService.Value.AuthenticateAsync(new AuthenticationRequestDto { Domain = request.Domain, Password = request.Password!, Username = request.Username! });
                if (authenticateResult.Data?.User is null)
                {
                    return Ok(new ApiResponse<AuthenticationResponseViewModel>
                    {
                        Errors = authenticateResult.Errors,
                    });
                }

                var signInResult = await identityService.Value.SignInAsync(new SignInRequestDto { RememberMe = request.RememberMe, User = authenticateResult.Data.User });
                return signInResult.OperationResult is not OperationResult.Succeeded
                    ? Ok(new ApiResponse<AuthenticationResponseViewModel>
                    {
                        Errors = signInResult.Errors,
                    })
                    : Ok(new ApiResponse<AuthenticationResponseViewModel>
                    {
                        Data = new AuthenticationResponseViewModel
                        {
                            Roles = signInResult.Data?.Roles?.ListToFlagsEnum<Role>(),
                        },
                    });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<AuthenticationResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpGet("[action]"), Produces(typeof(ApiResponse<Void>))]
        [Permission(policy: null)]
        [Display(Name = "خروج کاربر")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var result = await identityService.Value.SignOutAsync();

                return Ok(new ApiResponse<Void>
                {
                    Data = result.Data,
                    Errors = result.Errors,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<Void> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPut("[action]"), Produces(typeof(ApiResponse<Void>))]
        [Permission(policy: null)]
        [Display(Name = "تغییر رمز عبور")]
        public async Task<IActionResult> ChangePassword([NotNull] ChangePasswordRequestViewModel request)
        {
            try
            {
                var result = await identityService.Value.ChangePasswordAsync(new ChangePasswordRequestDto
                {
                    CurrentPassword = request.CurrentPassword,
                    NewPassword = request.NewPassword,
                });
                return Ok(new ApiResponse<Void>
                {
                    Errors = result.Errors,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<Void> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPost("token"), Produces(typeof(ApiResponse<GenerateTokenResponseViewModel>))]
        [Display(Name = "دریافت توکن")]
        public async Task<IActionResult> GenerateToken([NotNull] GenerateTokenRequestViewModel request)
        {
            try
            {
                var authenticateResult = await identityService.Value.AuthenticateAsync(new AuthenticationRequestDto { Password = request.Password!, Username = request.Username! });
                if (authenticateResult.Data?.User is null)
                {
                    return Ok(new ApiResponse<GenerateTokenResponseViewModel>
                    {
                        Errors = authenticateResult.Errors,
                    });
                }
                var signInResult = await identityService.Value.SignInAsync(new SignInRequestDto { RememberMe = false, User = authenticateResult.Data.User });
                if (signInResult.OperationResult is not OperationResult.Succeeded)
                {
                    return Ok(new ApiResponse<GenerateTokenResponseViewModel>
                    {
                        Errors = signInResult.Errors,
                    });
                }
                var result = await identityService.Value.GenerateUserTokenAsync(new GenerateUserTokenRequestDto
                {
                    UserId = authenticateResult.Data.User.Id,
                    TokenProvider = PermissionConstants.ApiDataProtectorTokenProvider,
                    Purpose = PermissionConstants.ApiDataProtectorTokenProviderAccessToken,
                });
                return Ok(new ApiResponse<GenerateTokenResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new GenerateTokenResponseViewModel
                    {
                        Token = result.Data?.Token,
                        ExpirationTime = result.Data?.ExpirationTime,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<GenerateTokenResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPost("tokens/revoke"), Produces(typeof(ApiResponse<RevokeTokenResponseViewModel>))]
        [Permission(policy: null)]
        [Display(Name = "منقضی کردن توکن")]
        public async Task<IActionResult> RevokeToken()
        {
            try
            {
                var result = await identityService.Value.RemoveUserTokenAsync(new RemoveUserTokenRequestDto
                {
                    UserId = User.UserId<int>(),
                    TokenProvider = PermissionConstants.ApiDataProtectorTokenProvider,
                    Purpose = PermissionConstants.ApiDataProtectorTokenProviderAccessToken,
                });
                return Ok(new ApiResponse<RevokeTokenResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new RevokeTokenResponseViewModel()
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<RevokeTokenResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpGet("[action]"), Produces(typeof(ApiResponse<bool>))]
        [Display(Name = "وضعیت احراز هویت")]
        public IActionResult Authenticated()
        {
            try
            {
                return Ok(new ApiResponse<bool>
                {
                    Data = User.Identity?.IsAuthenticated is true,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpGet("profiles"), Produces(typeof(ApiResponse<ProfileSettingsResponseViewModel>))]
        [Permission(policy: null)]
        [Display(Name = "دریافت تنظیمات پروفایل کاربر")]
        public async Task<IActionResult> GetProfileSettings()
        {
            try
            {
                var result = await identityService.Value.GetProfileSettingsAsync();

                return Ok(new ApiResponse<ProfileSettingsResponseViewModel>
                {
                    Data = new ProfileSettingsResponseViewModel
                    {
                        TimeZoneId = result.Data?.TimeZoneId,
                    },
                    Errors = result.Errors,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ProfileSettingsResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPut("profiles"), Produces(typeof(ApiResponse<Void>))]
        [Permission(policy: null)]
        [Display(Name = "بروز رسانی تنظیمات پروفایل کاربر")]
        public async Task<IActionResult> UpdateProfileSettings([NotNull] ProfileSettingsRequestViewModel request)
        {
            try
            {
                var result = await identityService.Value.UpdateProfileSettingsAsync(new ProfileSettingsDto
                {
                    TimeZoneId = request.TimeZoneId,
                });

                return Ok(new ApiResponse<Void>
                {
                    Errors = result.Errors,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<Void> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }
    }
}
