using Farsica.Template.Entity.Entities.Identity;

using System.Threading.Tasks;

namespace Farsica.Template
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Farsica.Framework.Hosting.Host.RunAsync<Startup, ApplicationUser, ApplicationRole>(args);
        }
    }
}