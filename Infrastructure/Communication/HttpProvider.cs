using Farsica.Template.Entity.DTOs.Communication;
using Farsica.Template.Shared.Infrastructure.Communication;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Farsica.Template.Infrastructure.Communication
{
    public class HttpProvider: IHttpProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpProvider> _logger;
        public HttpProvider(IHttpClientFactory httpClientFactory, ILogger<HttpProvider> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }


        public async Task<TResponse> PostAsync<TResponse, TBody>(HttpProviderRequest<TBody> request)
            where TResponse : class
        {
            try
            {
                var client = _httpClientFactory.CreateClient(Common.Constants.HttpClientIgnoreSsl);
                client.BaseAddress = new Uri(request.BaseAddress);
                if (request.HeaderParameters?.Any() == true)
                {
                    for (int i = 0; i < request.HeaderParameters.Count; i++)
                    {
                        client.DefaultRequestHeaders.Add(request.HeaderParameters[i].Key, request.HeaderParameters[i].Value);
                    }
                }
                var response = await client.PostAsJsonAsync(request.Uri, request.Body);
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PostAsync");

                throw;
            }
        }


        public async Task<TResponse> GetAsync<TResponse>(HttpProviderRequest<dynamic> request)
            where TResponse : class
        {
            return await GetAsync<TResponse, dynamic>(request);
        }

        public async Task<TResponse> GetAsync<TResponse, TBody>(HttpProviderRequest<TBody> request)
            where TResponse : class
        {
            try
            {
                var client = _httpClientFactory.CreateClient(Common.Constants.HttpClientIgnoreSsl);
                client.BaseAddress = new Uri(request.BaseAddress);
                if (request.HeaderParameters?.Any() == true)
                {
                    for (int i = 0; i < request.HeaderParameters.Count; i++)
                    {
                        client.DefaultRequestHeaders.Add(request.HeaderParameters[i].Key, request.HeaderParameters[i].Value);
                    }
                }

                if (request.Body != null)
                {
                    var body = System.Text.Json.JsonSerializer.Serialize(request.Body);

                    var type = body.GetType();
                    var props = type.GetProperties();
                    var pairs = props.Select(x => x.Name + "=" + x.GetValue(body, null)).ToArray();
                    string result = string.Join("&", pairs);

                    request.Uri += result;
                }
                var response = await client.GetAsync(request.Uri);
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAsync");

                throw;
            }
        }
    }
}
