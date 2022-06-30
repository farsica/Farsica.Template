using Farsica.Framework.DataAccess.Query;
using Farsica.Framework.DataAccess.UnitOfWork;
using Farsica.Framework.Mapping;
using Farsica.Framework.Service;
using Farsica.Template.Entity.Entities.Identity;
using Farsica.Template.Shared.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace Farsica.Template.DomainService
{
    public class UserService : ServiceBase<UserService>, IUserService
    {
        public UserService(IUnitOfWorkProvider unitOfWorkProvider, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger)
            : base(unitOfWorkProvider, httpContextAccessor, logger)
        {
        }

        public async Task<IList> GetUsers()
        {
            using var uow = UnitOfWorkProvider.CreateUnitOfWork();
            var repository = uow.GetRepository<ApplicationUser>();
            var includes = new Includes<ApplicationUser>(t =>
            {
                return t.Include(e => e.UserRoles);
            });
            var lst = await repository.GetAllAsync(includes: includes.Expression);

            var lst2 = await repository.GetManyQueryable(t => true).Select(t => t.AdaptData<ApplicationUser, ApplicationUserDto>()).ToListAsync();

            Logger.LogWarning("users readed");
            return lst.ToList();
        }

    }
}
