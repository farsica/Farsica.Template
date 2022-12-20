namespace Farsica.Template.Infrastructure.EntityFramework.Context
{
	using Farsica.Framework.DataAnnotation;
	using Farsica.Template.Data.Entity.Identity;
	using Farsica.Template.Shared.Infrastructure.DbContext;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Reflection;

	[ServiceLifetime(ServiceLifetime.Transient, "System.IServiceProvider,System.ComponentModel")]
	public class ApplicationDBContext : Framework.DataAccess.Context.EntityContextBase<ApplicationDBContext, ApplicationUser, ApplicationRole,
		long, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>, IDbContext
	{
		public ApplicationDBContext(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}

		protected override Assembly EntityAssembly => typeof(ApplicationUser).Assembly;

		protected override void OnConfiguring([NotNull] DbContextOptionsBuilder optionsBuilder)
		{
			_ = optionsBuilder.EnableSensitiveDataLogging(SensitiveDataLoggingEnabled)
				.EnableDetailedErrors(DetailedErrorsEnabled)
				.UseLoggerFactory(LoggerFactory)
				.UseSqlServer(ConnectionName);
			base.OnConfiguring(optionsBuilder);
		}
	}
}