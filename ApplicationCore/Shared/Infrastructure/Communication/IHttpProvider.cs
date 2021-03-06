using Farsica.Template.Entity.DTOs.Communication;
using Farsica.Framework.DataAnnotation;
using System.Threading.Tasks;

namespace Farsica.Template.Shared.Infrastructure.Communication
{
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
