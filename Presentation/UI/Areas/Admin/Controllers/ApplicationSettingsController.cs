namespace Farsica.Template.UI.Web.Areas.Admin.Controllers
{
    using Farsica.Template.Data.Dto.ApplicationSettings;
    using Farsica.Template.Data.Enumeration;
    using Farsica.Template.Data.ViewModel.ApplicationSettings;
    using Farsica.Template.Shared.Service;

    using Asp.Versioning;

    using Farsica.Framework.Core;
    using Farsica.Framework.Data;
    using Farsica.Framework.DataAnnotation;
    using Farsica.Framework.Identity;
    using Farsica.Framework.Mapping;

    using Microsoft.AspNetCore.Mvc;

    using System.Diagnostics.CodeAnalysis;

    [Framework.DataAnnotation.Area(nameof(Role.Admin), "مدیریت")]
    [Route("api/v{version:apiVersion}/[area]/[controller]")]
    [ApiVersion("1.0")]
    [Permission(Roles = [nameof(Role.Admin)])]
    [Display(Name = "تنظیمات سیستم")]
    public class ApplicationSettingsController(Lazy<ILogger<ApplicationSettingsController>> logger, Lazy<IApplicationSettingsService> applicationSettingsService)
        : ApiControllerBase<ApplicationSettingsController>(logger)
    {
        [HttpGet, Produces(typeof(ApiResponse<ApplicationSettingsViewModel>))]
        [Display(Name = "صفحه لیست تنظیمات سیستم")]
        public async Task<IActionResult> GetSettings()
        {
            try
            {
                var result = await applicationSettingsService.Value.GetApplicationSettingsAsync();
                return Ok(new ApiResponse<ApplicationSettingsViewModel>
                {
                    Errors = result.Errors,
                    Data = result.Data?.AdaptData<ApplicationSettingsDto, ApplicationSettingsViewModel>()
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ApplicationSettingsViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPut, Produces(typeof(ApiResponse<Void>))]
        [Display(Name = "ویرایش تنظیمات سیستم")]
        public async Task<IActionResult> UpdateSettings([NotNull] ApplicationSettingsViewModel request)
        {
            try
            {
                var result = await applicationSettingsService.Value.ModifyApplicationSettingsAsync(request.AdaptData<ApplicationSettingsViewModel, ApplicationSettingsDto>());
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
