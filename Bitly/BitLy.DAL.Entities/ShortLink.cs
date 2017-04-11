using System;

namespace BitLy.DAL.Entities
{
    /// <summary>
    /// Укороченная ссылка
    /// </summary>
    public class ShortLink
    {
        /// <summary>
        /// Уникальный идентификатор укороченной ссылки.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Оригинальная ссылка до конвертации.
        /// </summary>
        public string NativeUrl { get; set; }

        /// <summary>
        /// Короткая ссылка после конвертации.
        /// </summary>
        public string ShortUrl { get; set; }

        /// <summary>
        /// Активна ли ссылка? (может быть удалена из-за долгой неактивности) Предусмотреть //ToDo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата создания ссылки
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
