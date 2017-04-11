using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LinksFactory;

namespace Bitly.Controllers
{
    public class OpenLinkController : ApiController
    {
        /// <summary>
        /// Редирект на оригинальную ссылку
        /// </summary>
        [Route("l/{url}")]
        public async Task<HttpResponseMessage> GetLonglUrl([FromUri] string url)
        {
            var response = Request.CreateResponse(HttpStatusCode.Found);
            var longUrl = await LinkRedirector.OpenShortUrl(url).ConfigureAwait(false);
            response.Headers.Location = new Uri(longUrl);
            return response;
        }

        /// <summary>
        /// Редирект на статистику
        /// </summary>
        [Route("s/{url}")]
        public HttpResponseMessage GetStatistics([FromUri] string url)
        {
            var response = Request.CreateResponse(HttpStatusCode.Found);
            response.Headers.Location = new Uri("/");
            return response;
        }
    }
}