namespace Farsica.Template.Shared.Service
{
	using Farsica.Framework.DataAnnotation;
	using System.Collections;
	using System.Threading.Tasks;

	[Injectable]
	public interface IUserService
	{
		Task<Framework.Data.ResultData<IEnumerable>> GetUsers();
	}
}
