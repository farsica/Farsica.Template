using Farsica.Template.Entity.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;

namespace Farsica.Template.UI.Web.Api
{
	public class ServiceController : Farsica.Framework.Core.ApiControllerBase<ServiceController, ApplicationUser>
	{
		public ServiceController(Lazy<UserManager<ApplicationUser>> userManager, Lazy<ILogger<ServiceController>> logger, Lazy<IStringLocalizer<ServiceController>> localizer) 
			: base(userManager, logger, localizer)
		{
		}
	}
}
