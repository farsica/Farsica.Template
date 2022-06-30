using Farsica.Framework.DataAccess.UnitOfWork;
using Farsica.Framework.Service;
using Farsica.Template.Shared.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Farsica.Template.DomainService
{
    public class GlobalService : ServiceBase<GlobalService>, IGlobalService
    {
        public GlobalService(IUnitOfWorkProvider unitOfWorkProvider, IHttpContextAccessor httpContextAccessor, ILogger<GlobalService> logger)
            : base(unitOfWorkProvider, httpContextAccessor, logger)
        {
        }
    }
}
