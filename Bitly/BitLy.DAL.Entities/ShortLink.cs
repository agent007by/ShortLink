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
        public int LinkId { get; set; }
        /// <summary>
        /// Оригинальная ссылка до конвертации.
        /// </summary>
        public string LongUrl { get; set; }
        /// <summary>
        /// Короткая ссылка после конвертации.
        /// </summary>
        public string ShortUrl { get; set; }
    }
}
