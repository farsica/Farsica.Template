using Farsica.Template.Entity.Entities.Identity;
using Farsica.Framework.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Farsica.Template
{
    public class Startup : Startup<ApplicationUser, ApplicationRole>
    {
        public Startup(IConfiguration configuration) :
            base(configuration, defaultNamespace: "Farsica.Template", exceptionHandlerOptions: null, localization: false, authentication: false, razorPages: false, antiforgery: false, https: false, views: false, identity: false)
        {
        }

        protected override void ConfigureServicesCore(IServiceCollection services, IMvcBuilder mvcBuilder)
        {
        }

        protected override void ConfigureCore(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
