﻿namespace Farsica.Template.Shared.Service
{
    using Farsica.Template.Data.Dto.Identity;
    using Farsica.Template.Data.Entity.Identity;

    using Farsica.Framework.Data;
    using Farsica.Framework.DataAccess.Specification;
    using Farsica.Framework.DataAnnotation;

    using Microsoft.AspNetCore.Authentication.Cookies;

    using System.Diagnostics.CodeAnalysis;

    [Injectable]
    public interface IIdentityService
    {
        Task<ResultData<ListDataSource<ApplicationUserDto>>> GetUsersAsync(ListRequestDto<ApplicationUser>? requestDto = null);

        Task<ResultData<IEnumerable<ApplicationRoleDto>>> GetRolesAsync(ISpecification<ApplicationRoleDto>? specification = null);

        Task<ResultData<ApplicationUserDto>> GetUserAsync([NotNull] ISpecification<ApplicationUser> specification);

        Task<ResultData<ICollection<string>>> GetUserRolesAsync([NotNull] int userId);

        Task<ResultData<AuthenticationResponseDto>> AuthenticateAsync([NotNull] AuthenticationRequestDto requestDto);

        Task<ResultData<SignInResponseDto>> SignInAsync([NotNull] SignInRequestDto requestDto);

        Task<ResultData<Void>> SignOutAsync();

        Task<ResultData<bool>> CreateUserAsync([NotNull] CreateUserRequestDto requestDto);

        Task<ResultData<bool>> UpdateUserAsync([NotNull] UpdateUserRequestDto requestDto);

        Task<ResultData<bool>> ToggleUserAsync([NotNull] ISpecification<ApplicationUser> specification);

        Task<ResultData<bool>> RemoveUserAsync([NotNull] ISpecification<ApplicationUser> specification);

        Task<ResultData<string?>> GetUserTokenAsync([NotNull] GetUserTokenRequestDto requestDto);

        Task<ResultData<GenerateUserTokenResponseDto>> GenerateUserTokenAsync([NotNull] GenerateUserTokenRequestDto requestDto);

        Task<ResultData<bool>> RemoveUserTokenAsync([NotNull] RemoveUserTokenRequestDto requestDto);

        Task<ResultData<bool>> ChangePasswordAsync([NotNull] ChangePasswordRequestDto requestDto);

        Task<ResultData<bool>> ResetPasswordAsync([NotNull] ResetPasswordRequestDto requestDto);

        Task ValidatePrincipalAsync([NotNull] CookieValidatePrincipalContext context);

        Task<ResultData<UserPermissionsResponseDto>> GetUserPermissionsAsync([NotNull] UserPermissionsRequestDto requestDto);

        Task<ResultData<Void>> UpdateUserPermissionsAsync([NotNull] UpdateUserPermissionsRequestDto requestDto);

        Task<ResultData<ProfileSettingsDto>> GetProfileSettingsAsync();

        Task<ResultData<Void>> UpdateProfileSettingsAsync([NotNull] ProfileSettingsDto requestDto);
    }
}
