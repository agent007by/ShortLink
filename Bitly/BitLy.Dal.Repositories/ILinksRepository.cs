using System.Collections.Generic;
using System.Threading.Tasks;
using BitLy.DAL.Entities;

namespace BitLy.DAL.Repositories
{
    /// <summary>
    /// Репозиторий для работы с ссылками
    /// </summary>
    public interface ILinkRepository
    {
        /// <summary>
        /// Сохраняет ссылку и ее укороченный вариант в БД.
        /// </summary>
        Task SaveLink(string shortLink, string longLink);

        /// <summary>
        ///Получение информации о ссылке по ShortLink
        /// </summary>
        Task<ShortLink> GetNativeLink(string shortLink);

        /// <summary>
        /// Получение информации о ссылках
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ShortLink>> GetNativeLinks();

        /// <summary>
        ///Получение информации о ссылке по ShortLink из кеша
        /// </summary>
        Task<ShortLink> GetNativeLinkCachedAsync(string shortLink);

        /// <summary>
        /// Сохраняем счетчики открытия ссылок
        /// </summary>
        /// <param name="linksRedirectsCount">Словарь ссылок и колличество их открытий</param>
        Task SaveOpenLinksCountAsync(IDictionary<int, int> linksRedirectsCount);

        /// <summary>
        /// Получение общей статистики переходов по ссылкам
        /// </summary>
        Task<IEnumerable<ShortLinkStatistics>> GetShortLinksOpenStatisticsAsync();


    }
}
