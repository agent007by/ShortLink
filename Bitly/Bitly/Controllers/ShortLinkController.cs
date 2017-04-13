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
        public async Task<IHttpActionResult> CreateNewShortUrl([FromUri] string nativeUrl)
        {
            var link = await LinksGenerator.GetNewShortLinkAsync(nativeUrl);
            return Ok(link);
        }

        /// <summary>
        /// Получение статистики переходов по коротким ссылкам
        /// </summary>
        [Route("api/shortUrl/statistics/")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStatistics()
        {
            var statistics = await LinkStatisticCounter.GetStatistics();
            return Ok(statistics);
        }

        /// <summary>
        /// Редирект на оригинальную ссылку
        /// </summary>
        [Route("{url}")]
        public async Task<HttpResponseMessage> GetLonglUrl([FromUri] string url)
        {
            //TODO укоротить Url убрать "l"
            //StatusCode =HttpStatusCode.NotFound
            var response = Request.CreateResponse(HttpStatusCode.Found);
            var longUrl = await LinkRedirector.OpenShortUrl(url).ConfigureAwait(false);

            if (longUrl != null && longUrl.Length > 0 && longUrl.Contains("//"))
            {
                response.Headers.Location = new Uri(longUrl);
            }
            return response;
        }
    }
}