namespace Farsica.Template.Shared.Infrastructure.Communication
{
	using Farsica.Template.Data.Dto.Communication;
	using Farsica.Framework.DataAnnotation;
	using System.Threading.Tasks;

	[Injectable]
	public interface IHttpProvider
	{
		Task<TResponse> PostAsync<TResponse, TBody>(HttpProviderRequest<TBody> request)
			   where TResponse : class;
		Task<TResponse> GetAsync<TResponse, TBody>(HttpProviderRequest<TBody> request)
			where TResponse : class;
		Task<TResponse> GetAsync<TResponse>(HttpProviderRequest<dynamic> request)
			where TResponse : class;
	}
}
