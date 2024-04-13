namespace Farsica.Template.UI.Web.Areas.Admin.Controllers
{
    using Farsica.Template.Data.Dto.Identity;
    using Farsica.Template.Data.Entity.Identity;
    using Farsica.Template.Data.Enumeration;
    using Farsica.Template.Data.ViewModel.Identity;
    using Farsica.Template.Shared.Service;

    using Asp.Versioning;

    using Farsica.Framework.Core;
    using Farsica.Framework.Data;
    using Farsica.Framework.DataAccess.Specification.Impl;
    using Farsica.Framework.DataAnnotation;
    using Farsica.Framework.Identity;
    using Farsica.Framework.Mvc.Routing;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Dynamic.Core;

    using static Farsica.Framework.Core.Constants;

    [Framework.DataAnnotation.Area(nameof(Role.Admin), "مدیریت")]
    [Route("api/v{version:apiVersion}/[area]/[controller]")]
    [ApiVersion("1.0")]
    [Permission(Roles = [nameof(Role.Admin)])]
    [Display(Name = "کاربران")]
    public class IdentitiesController(Lazy<ILogger<IdentitiesController>> logger, Lazy<IIdentityService> identityService, Lazy<IEndpointDataSource> endpointDataSource, Lazy<IStringLocalizer<IdentitiesController>> localizer) : LocalizableApiControllerBase<IdentitiesController>(logger, localizer)
    {
        [HttpGet, Produces(typeof(ApiResponse<ListDataSource<UserListResponseViewModel>>))]
        [Display(Name = "لیست کاربران")]
        public async Task<IActionResult> GetUsers([NotNull, FromQuery] ConsumersRequestViewModel request)
        {
            try
            {
                var result = await identityService.Value.GetUsersAsync(new ListRequestDto<ApplicationUser> { PagingDto = request.PagingDto });
                return Ok(new ApiResponse<ListDataSource<UserListResponseViewModel>>
                {
                    Errors = result.Errors,
                    Data = result.Data.List is null ? new() : new ListDataSource<UserListResponseViewModel>
                    {
                        List = result.Data.List.Select(t => new UserListResponseViewModel
                        {
                            Id = t.Id,
                            Username = t.UserName,
                            Email = t.Email,
                            PhoneNumber = t.PhoneNumber,
                            Enabled = t.Enabled,
                        }),
                        TotalRecordsCount = result.Data.TotalRecordsCount,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ListDataSource<UserListResponseViewModel>> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpGet("{userId:int}"), Produces(typeof(ApiResponse<UserResponseViewModel>))]
        [Display(Name = "جزئیات کاربر")]
        public async Task<IActionResult> Get([FromRoute] int userId)
        {
            try
            {
                var result = await identityService.Value.GetUserAsync(new IdEqualsSpecification<ApplicationUser, int>(userId));
                return Ok(new ApiResponse<UserResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = result.Data is null ? null : new UserResponseViewModel
                    {
                        Id = result.Data.Id,
                        Username = result.Data.UserName,
                        Email = result.Data.Email,
                        PhoneNumber = result.Data.PhoneNumber,
                        Enabled = result.Data.Enabled,
                        RegistrationDate = result.Data.RegistrationDate,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<UserResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPost, Produces(typeof(ApiResponse<Void>))]
        [Display(Name = "تعریف کاربر")]
        public async Task<IActionResult> Create([NotNull] ManageUserRequestViewModel request)
        {
            try
            {
                var result = await identityService.Value.CreateUserAsync(new CreateUserRequestDto
                {
                    Username = request.Username,
                    Password = request.Password,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
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

        [HttpPut("{userId:int}"), Produces(typeof(ApiResponse<Void>))]
        [Display(Name = "ویرایش کاربر")]
        public async Task<IActionResult> Update([FromRoute] int userId, [NotNull, FromBody] ManageUserRequestViewModel request)
        {
            try
            {
                var result = await identityService.Value.UpdateUserAsync(new UpdateUserRequestDto
                {
                    Id = userId,
                    Username = request.Username,
                    Password = request.Password,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
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

        [HttpDelete("{userId:int}"), Produces(typeof(ApiResponse<bool>))]
        [Display(Name = "حذف کاربر")]
        public async Task<IActionResult> Remove([FromRoute] int userId)
        {
            try
            {
                var result = await identityService.Value.RemoveUserAsync(new IdEqualsSpecification<ApplicationUser, int>(userId));
                return Ok(new ApiResponse<bool>
                {
                    Errors = result.Errors,
                    Data = result.Data
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPatch("{userId:int}/toggle"), Produces(typeof(ApiResponse<Void>))]
        [Display(Name = "فعال/غیر فعال کردن کاربر")]
        public async Task<IActionResult> Toggle([FromRoute] int userId)
        {
            try
            {
                var result = await identityService.Value.ToggleUserAsync(new IdEqualsSpecification<ApplicationUser, int>(userId));
                return Ok(new ApiResponse<AuthenticationResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new AuthenticationResponseViewModel()
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<Void> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpGet("{userId:int}/token"), Produces(typeof(ApiResponse<GetTokenResponseViewModel>))]
        [Display(Name = "مشاهده توکن کاربر")]
        public async Task<IActionResult> GetUserToken([FromRoute] int userId)
        {
            try
            {
                var result = await identityService.Value.GetUserTokenAsync(new GetUserTokenRequestDto
                {
                    UserId = userId,
                    TokenProvider = PermissionConstants.ApiDataProtectorTokenProvider,
                    Purpose = PermissionConstants.ApiDataProtectorTokenProviderAccessToken,
                });
                return Ok(new ApiResponse<GetTokenResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new GetTokenResponseViewModel
                    {
                        Token = result.Data,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<GetTokenResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPost("{userId:int}/token"), Produces(typeof(ApiResponse<GenerateTokenResponseViewModel>))]
        [Display(Name = "ایجاد توکن کاربر")]
        public async Task<IActionResult> GenerateToken([FromRoute] int userId)
        {
            try
            {
                var result = await identityService.Value.GenerateUserTokenAsync(new GenerateUserTokenRequestDto
                {
                    UserId = userId,
                    TokenProvider = PermissionConstants.ApiDataProtectorTokenProvider,
                    Purpose = PermissionConstants.ApiDataProtectorTokenProviderAccessToken,
                });
                return Ok(new ApiResponse<GenerateTokenResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new GenerateTokenResponseViewModel
                    {
                        Token = result.Data?.Token,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<GenerateTokenResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpDelete("{userId:int}/token"), Produces(typeof(ApiResponse<bool>))]
        [Display(Name = "حذف توکن کاربر")]
        public async Task<IActionResult> RemoveToken([FromRoute] int userId)
        {
            try
            {
                var result = await identityService.Value.RemoveUserTokenAsync(new RemoveUserTokenRequestDto
                {
                    UserId = userId,
                    TokenProvider = PermissionConstants.ApiDataProtectorTokenProvider,
                    Purpose = PermissionConstants.ApiDataProtectorTokenProviderAccessToken,
                });
                return Ok(new ApiResponse<bool>
                {
                    Errors = result.Errors,
                    Data = result.OperationResult is OperationResult.Succeeded,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPut("{userId:int}/[action]"), Produces(typeof(ApiResponse<Void>))]
        [Display(Name = "تنظیم مجدد رمز عبور")]
        public async Task<IActionResult> ResetPassword([FromRoute] int userId, [NotNull] ResetPasswordRequestViewModel request)
        {
            try
            {
                var userResult = await identityService.Value.GetUserAsync(new IdEqualsSpecification<ApplicationUser, int>(userId));
                if (userResult.OperationResult is not OperationResult.Succeeded)
                {
                    return Ok(new ApiResponse<Void>
                    {
                        Errors = userResult.Errors,
                    });
                }
                var resetPasswordResult = await identityService.Value.ResetPasswordAsync(new ResetPasswordRequestDto
                {
                    UserId = userId,
                    NewPassword = request.NewPassword,
                });
                return Ok(new ApiResponse<Void>
                {
                    Errors = resetPasswordResult.Errors,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<Void> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpGet("{userId:int}/[action]"), Produces(typeof(ApiResponse<UserPermissionsResponseViewModel>))]
        [Display(Name = "لیست دسترسی های کاربر")]
        public async Task<IActionResult> Permissions([FromRoute] int userId)
        {
            try
            {
                var endpoints = endpointDataSource.Value.GetEndpoints();
                if (endpoints is null)
                {
                    return Ok(new ApiResponse<UserPermissionsResponseViewModel>());
                }

                var result = await identityService.Value.GetUserPermissionsAsync(new UserPermissionsRequestDto { UserId = userId });

                var areas = endpoints.Where(t => t.Controller.HasValue).GroupBy(t => t.Area);
                List<ClaimsResponseViewModel> items = new(areas.Count());
                foreach (var area in areas)
                {
                    var controllers = area.GroupBy(t => t.Controller);
                    var areaItem = new ClaimsResponseViewModel
                    {
                        Text = area.Key?.Value ?? Localizer.Value["Root"],
                        Items = controllers.Select(t => new ClaimsResponseViewModel
                        {
                            Text = t.Key?.Value,
                            Items = t.Select(c => new ClaimsResponseViewModel
                            {
                                Text = c.Action?.Value,
                                Value = c.EndpointName,
                                HasPermission = result.Data?.Claims?.Contains(c.EndpointName) is true,
                            }),
                        }),
                    };

                    items.Add(areaItem);
                }

                return Ok(new ApiResponse<UserPermissionsResponseViewModel>
                {
                    Data = new UserPermissionsResponseViewModel
                    {
                        Claims = items,
                        Roles = result.Data?.Roles,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ListDataSource<ClaimsResponseViewModel>> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPut("{userId:int}/[action]"), Produces(typeof(ApiResponse<Void>))]
        [Display(Name = "ویرایش دسترسی های کاربر")]
        public async Task<IActionResult> ManageUserPermissions([FromRoute] int userId, [NotNull] ManageUserPermissionsRequestViewModel request)
        {
            try
            {
                var result = await identityService.Value.UpdateUserPermissionsAsync(new UpdateUserPermissionsRequestDto
                {
                    UserId = userId,
                    Claims = request.Claims,
                    Roles = request.Roles
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
