using System;

namespace BitLy.DAL.Entities
{
    /// <summary>
    /// Статистический эллемент открытия ссылки в еденицу времени
    /// </summary>
    public class ShortLinkOpenCountStatistic
    {
        /// <summary>
        /// Дата открытия ссылки
        /// </summary>
        public DateTime OpenDate { get; set; }

        /// <summary>
        /// Количество открытий
        /// </summary>
        public int OpenCount { get; set; }
    }
}
