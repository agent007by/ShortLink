using System;

namespace BitLy.DAL.Entities
{
    /// <summary>
    /// Статистический эллемент открытия ссылки в еденицу времени
    /// </summary>
    public class ShortLinkRedirectCountStatistics
    {
        /// <summary>
        /// Id ссылки
        /// </summary>
        public int ShortLinkId { get; set; }
        
        /// <summary>
        /// Дата открытия ссылки
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Количество открытий
        /// </summary>
        public int RedirectCount { get; set; }
    }
}
