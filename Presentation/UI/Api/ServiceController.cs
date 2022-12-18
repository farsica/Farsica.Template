namespace Farsica.Template.UI.Web.Api
{
	using Farsica.Template.Entity.Entities.Identity;
	using Mapster;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.Localization;
	using Microsoft.Extensions.Logging;
	using System;
	using System.Threading.Tasks;

	public class ServiceController : Framework.Core.ApiControllerBase<ServiceController, ApplicationUser>
	{
		public ServiceController(Lazy<UserManager<ApplicationUser>> userManager, Lazy<ILogger<ServiceController>> logger, Lazy<IStringLocalizer<ServiceController>> localizer)
			: base(userManager, logger, localizer)
		{
		}

		public async Task<ApplicationUserDto> GetUsers()
		{
			var user = await UserManager.Value.FindByNameAsync("test");
			return user.Adapt<ApplicationUserDto>();
		}
	}
}
