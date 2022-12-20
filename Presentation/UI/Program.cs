namespace Farsica.Template
{
	using Farsica.Template.Data.Entity.Identity;

	using System.Threading.Tasks;

	public static class Program
	{
		public static async Task Main(string[] args)
		{
			await Framework.Hosting.Host.RunAsync<Startup, ApplicationUser, ApplicationRole>(args);
		}
	}
}