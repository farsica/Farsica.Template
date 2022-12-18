namespace Farsica.Template.DomainService
{
	using Farsica.Framework.DataAccess.Query;
	using Farsica.Framework.DataAccess.UnitOfWork;
	using Farsica.Framework.Mapping;
	using Farsica.Framework.Service;
	using Farsica.Template.Entity.Entities.Identity;
	using Farsica.Template.Shared.Service;
	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;
	using System;
	using System.Collections;
	using System.Linq;
	using System.Threading.Tasks;
	using static Farsica.Framework.Core.Constants;

	public class UserService : ServiceBase<UserService>, IUserService
	{
		public UserService(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<ILogger<UserService>> logger)
			: base(unitOfWorkProvider, httpContextAccessor, logger)
		{
		}

		public async Task<Framework.Data.ResultData<IEnumerable>> GetUsers()
		{
			using var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
			var repository = uow.GetRepository<ApplicationUser, long>();
			var includes = new Includes<ApplicationUser>(t => t.Include(e => e.UserRoles));
			var lst = await repository.GetAllAsync(includes: includes.Expression);

			var lst2 = await repository.GetManyQueryable(t => true).Select(t => t.AdaptData<ApplicationUser, ApplicationUserDto>()).ToListAsync();

			Logger.Value.LogWarning("users readed");

			return new(OperationResult.Succeeded) { Data = lst };
		}
	}
}
