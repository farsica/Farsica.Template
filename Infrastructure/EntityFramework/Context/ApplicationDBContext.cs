using Farsica.Framework.DataAnnotation;
using Farsica.Template.Entity.Entities.Identity;
using Farsica.Template.Shared.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Farsica.Template.Infrastructure.EntityFramework.Context
{
    [ServiceLifetime(ServiceLifetime.Transient, "System.IServiceProvider,System.ComponentModel")]
    public class ApplicationDBContext : Farsica.Framework.DataAccess.Context.EntityContextBase<ApplicationDBContext, ApplicationUser, ApplicationRole,
        int, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>, IDbContext
    {
        public ApplicationDBContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override Assembly EntityAssembly => typeof(ApplicationUser).Assembly;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging(SensitiveDataLoggingEnabled)
                .EnableDetailedErrors(DetailedErrorsEnabled)
                .UseLoggerFactory(LoggerFactory)
                .UseSqlServer(ConnectionName, (builder) =>
                {
                    builder.MigrationsHistoryTable(Farsica.Framework.Data.DbProviderFactories.GetFactory.GetObjectName("MIGRATIONS_HISTORY"), DefaultSchema);
                });
            base.OnConfiguring(optionsBuilder);
        }
    }
}