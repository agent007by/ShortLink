using System.Collections.Generic;

namespace BitLy.DAL.Entities
{
    /// <summary>
    /// Общая статистика открытия ссылок
    /// </summary>
    public class ShortLinkStatistics
    {
        /// <summary>
        /// Укороченная ссылка
        /// </summary>
        public ShortLink Link { get; set; }

        /// <summary>
        /// Суммарная статистика открытий ссылок
        /// </summary>
        public IEnumerable<ShortLinkRedirectCountStatistics> Statistics { get; set; }
    }
}
