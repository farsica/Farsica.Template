using Farsica.Template.Entity.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Farsica.Template.UI.Web.Api
{
    public class ServiceController : Farsica.Framework.Core.ApiControllerBase<ServiceController, ApplicationUser>
    {
        public ServiceController(UserManager<ApplicationUser> userManager, ILogger<ServiceController> logger, IStringLocalizer<ServiceController> localizer)
            : base(userManager, logger, localizer)
        {
        }
    }
}
