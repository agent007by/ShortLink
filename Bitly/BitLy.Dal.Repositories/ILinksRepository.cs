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
        /// Сохраняем счетчики открытия ссылок
        /// </summary>
        /// <param name="linksOpenCount">Словарь ссылок и колличество их открытий</param>
        Task SaveOpenLinksCountAsync(IDictionary<string, int> linksOpenCount);

        /// <summary>
        /// Получение общей статистики переходов по ссылкам
        /// </summary>
        Task<IEnumerable<ShortLinkOpenStatistics>> GetShortLinksOpenStatisticsAsync();

    }
}
