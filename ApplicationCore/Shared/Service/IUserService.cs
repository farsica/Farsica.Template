using Farsica.Framework.DataAnnotation;
using System.Collections;
using System.Threading.Tasks;

namespace Farsica.Template.Shared.Service
{
    [Injectable]
    public interface IUserService
    {
        Task<IList> GetUsers();
    }
}
