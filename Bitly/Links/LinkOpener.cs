using System.Threading.Tasks;
using BitLy.DAL.EF;

namespace LinksFactory
{
    public static class LinkOpener
    {
        /// <summary>
        /// Открытие длинной ссылки, через короткую, с засчитываем клика и т.д.
        ///  </summary>
        /// <param name="shortUrl"></param>
        /// <returns>Выдает длинную ссылку</returns>
        public static async Task<string> OpenShortUrl(string shortUrl)
        {
            var dbLinks = new DbLinks();
            await LinkStatisticCounter.AddOpenLinkCount(shortUrl).ConfigureAwait(false);
            var longUrl = await dbLinks.GetOriginalLinkCachedAsync(shortUrl).ConfigureAwait(false);
            return longUrl;
        }
    }
}
