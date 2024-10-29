using CareWebServiceEndpoint.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace CareWebServiceEndpoint.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class Upload_UP00000001 : Controller
    {
        [HttpPost("/Upload-Data")]
        public async Task<HttpResponseMessage> Upload_Data()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            var client = new HttpClient(handler);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://172.20.12.55/CareWebServiceV5/WSEUploader.asmx?op=Upload_Excel");
            request.Content = new StringContent(XDocument.Load("C:\\Users\\josaphat.cornelius\\Documents\\CWSEndpoint\\CareWebServiceEndpoint\\UP00000001_Template_Upload.txt").ToString(), System.Text.Encoding.UTF8, "application/soap+xml");

            var response = await client.SendAsync(request);

            return response;
        }
    }
}