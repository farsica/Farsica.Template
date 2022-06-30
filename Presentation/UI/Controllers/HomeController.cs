using Microsoft.AspNetCore.Mvc;
using Farsica.Template.Shared.Service;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Farsica.Template.Entity.Entities.Identity;
using Microsoft.Extensions.Localization;

namespace Farsica.Template.UI.Web.Controllers
{
    public class HomeController : Farsica.Framework.Core.ControllerBase<HomeController, ApplicationUser>
    {
        private readonly IUserService userService;
        private readonly SignInManager<ApplicationUser> signInManager;

        public HomeController(UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer, IUserService userService, SignInManager<ApplicationUser> signInManager)
            : base(userManager, logger, localizer)
        {
            this.userService = userService;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
