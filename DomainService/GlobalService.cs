namespace Farsica.Template.DomainService
{
	using Farsica.Framework.DataAccess.UnitOfWork;
	using Farsica.Framework.Service;
	using Farsica.Template.Shared.Service;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Logging;
	using System;

	public class GlobalService : ServiceBase<GlobalService>, IGlobalService
	{
		public GlobalService(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<ILogger<GlobalService>> logger)
			: base(unitOfWorkProvider, httpContextAccessor, logger)
		{
		}
	}
}
