using CareWebServiceEndpoint.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CareWebServiceEndpoint.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class Upload_UP00000001 : Controller
    {
        private static IHttpClientFactory _httpClientFactory;

        private static IHttpClientFactory GetHttpClientFactory()
        {
            if (_httpClientFactory != null)
            {
                return _httpClientFactory;
            }
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            return _httpClientFactory;
        }

        public static HttpClient GetClient()
        {
            return GetHttpClientFactory().CreateClient();
        }

        [HttpPost("/Upload-Data")]
        public async Task<HttpResponseMessage> Upload_Data([FromBody] UP00000001Model UP01)
        {
            var client = GetClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://172.20.12.55/CareWebServiceV5/WSEUploader.asmx?op=Upload_Excel");
            request.Content = new StringContent(JsonSerializer.Serialize(UP01), System.Text.Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            return response;
        }
    }
}