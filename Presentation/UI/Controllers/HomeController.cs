namespace Farsica.Template.UI.Web.Controllers
{
	using Farsica.Template.Entity.Entities.Identity;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Localization;
	using Microsoft.Extensions.Logging;
	using System;
	using System.Threading.Tasks;

	public class HomeController : Framework.Core.ControllerBase<HomeController, ApplicationUser>
	{
		public HomeController(Lazy<UserManager<ApplicationUser>> userManager, Lazy<ILogger<HomeController>> logger, Lazy<IStringLocalizer<HomeController>> localizer)
			: base(userManager, logger, localizer)
		{
		}

		public async Task<IActionResult> Index()
		{
			return View();
		}
	}
}
