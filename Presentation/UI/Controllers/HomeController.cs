namespace Farsica.Template.UI.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using System;

    public class HomeController(Lazy<ILogger<HomeController>> logger, Lazy<IWebHostEnvironment> environment) : Framework.Core.ControllerBase<HomeController>(logger)
    {
        public IActionResult Index() => environment.Value.IsDevelopment() ? Redirect("/swagger") : Ok();
    }
}
