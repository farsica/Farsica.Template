namespace Farsica.Template.UI.Web.Api
{
    using Farsica.Template.Data.ViewModel.Common;
    using Farsica.Template.Shared.Service;

    using Asp.Versioning;

    using Farsica.Framework.Core;
    using Farsica.Framework.Data;
    using Farsica.Framework.DataAnnotation;
    using Farsica.Framework.TimeZone;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Reflection;

    using Globals = Framework.Core.Globals;

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Display(Name = "عمومی")]
    public class CommonController(Lazy<ILogger<CommonController>> logger, Lazy<ITimeZoneProvider> timeZoneProvider, Lazy<IApplicationSettingsService> applicationSettingsService) : ApiControllerBase<CommonController>(logger)
    {
        [HttpGet("settings"), Produces(typeof(ApiResponse<SettingsViewModel>))]
        [AllowAnonymous]
        [Display(Name = "تنظیمات برنامه")]
        public async Task<IActionResult> Settings() => Ok(new ApiResponse<SettingsViewModel>
        {
            Data = new()
            {
                Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
                GridPageSize = (await applicationSettingsService.Value.GetApplicationSettingsAsync()).Data?.GridPageSize ?? 10,
            }
        });

        [HttpGet("[action]"), Produces(typeof(ApiResponse<dynamic>))]
        [AllowAnonymous]
        [Display(Name = "لیست ترجمه ها")]
        public IActionResult Resources()
        {
            try
            {
                return Ok(new ApiResponse<dynamic>
                {
                    Data = Globals.GetAllResourceString(Startup.DefaultNamespace),
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<dynamic> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpGet("[action]"), Produces(typeof(ApiResponse<ListDataSource<KeyValuePair<string?, string?>>>))]
        [AllowAnonymous]
        [Display(Name = "لیست ناحیه زمانی")]
        public IActionResult TimeZones()
        {
            var result = timeZoneProvider.Value.GetTimeZones();
            return Ok(new ApiResponse<ListDataSource<KeyValuePair<string?, string?>>>
            {
                Data = new ListDataSource<KeyValuePair<string?, string?>>
                {
                    List = result?.Select(t => new KeyValuePair<string?, string?>(t.Id, t.DisplayName))
                }
            });
        }
    }
}
