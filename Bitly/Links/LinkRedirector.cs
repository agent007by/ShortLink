using System.Threading.Tasks;
using BitLy.DAL.EF;

namespace LinksFactory
{
    public static class LinkRedirector
    {
        /// <summary>
        /// Открытие длинной ссылки, через короткую, с засчитываем клика и т.д.
        ///  </summary>
        /// <param name="shortUrl"></param>
        /// <returns>Выдает длинную ссылку</returns>
        public static async Task<string> OpenShortUrl(string shortUrl)
        {
            var dbLinks = new DbLinks();
            var link = await dbLinks.GetNativeLinkCachedAsync(shortUrl).ConfigureAwait(false);
            await LinkStatisticCounter.AddRedirectLinkCount(link.Id).ConfigureAwait(false);
            return link.NativeUrl;
        }
    }
}
