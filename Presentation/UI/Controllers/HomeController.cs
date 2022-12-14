using Microsoft.AspNetCore.Mvc;
using Farsica.Template.Shared.Service;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Farsica.Template.Entity.Entities.Identity;
using Microsoft.Extensions.Localization;
using Farsica.Template.UI.Web.Api;
using System;

namespace Farsica.Template.UI.Web.Controllers
{
    public class HomeController : Farsica.Framework.Core.ControllerBase<HomeController, ApplicationUser>
    {
        private readonly Lazy<IUserService> userService;
        private readonly Lazy<SignInManager<ApplicationUser>> signInManager;

        public HomeController(Lazy<UserManager<ApplicationUser>> userManager, Lazy<ILogger<HomeController>> logger, Lazy<IStringLocalizer<HomeController>> localizer, Lazy<IUserService> userService, Lazy<SignInManager<ApplicationUser>> signInManager)
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
