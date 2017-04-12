using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LinksFactory;

namespace Bitly.Controllers
{
    public class ShortLinkController : ApiController
    {
        /// <summary>
        /// Получение новой короткой ссылки
        /// </summary>
        [Route("api/shortUrl/new/")]
        [HttpPut]
        public async Task<IHttpActionResult> GetStatistics([FromUri] string nativeUrl)
        {
            //ToDo повесить backend
            return Ok("//sl.somee.com/l/12345");
        }

        /// <summary>
        /// Редирект на оригинальную ссылку
        /// </summary>
        [Route("l/{url}")]
        public async Task<HttpResponseMessage> GetLonglUrl([FromUri] string url)
        {
            //TODO укоротить Url убрать "l"
            var response = Request.CreateResponse(HttpStatusCode.Found);
            var longUrl = await LinkRedirector.OpenShortUrl(url).ConfigureAwait(false);
            response.Headers.Location = new Uri(longUrl);
            return response;
        }

        //ToDo добавить получение статистики
    }
}