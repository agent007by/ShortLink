using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitLy.DAL.Repositories;
using BitLy.DAL.Entities;
using BitLy.CacheHelper;

namespace BitLy.DAL.EF
{
    public class DbLinks : ILinkRepository
    {
        /// <summary>
        /// Получение общей статистики переходов по ссылкам
        /// </summary>
        public  async Task<IEnumerable<ShortLinkOpenStatistics>> GetShortLinksOpenStatisticsAsync()
        {
            return await Task.Run(() => new List<ShortLinkOpenStatistics> {
                new ShortLinkOpenStatistics
                {
                    Link = new ShortLink { LinkId = 1, LongUrl = "http://mail.ru", ShortUrl = "http://bitly/l/fqwerr" },
                    Statistics = new List<ShortLinkOpenCountStatistic> { new ShortLinkOpenCountStatistic { OpenCount = 100, OpenDate = DateTime.Now } }
                }
                });
        }


        /// <summary>
        /// Сохраняем счетчики открытия ссылок
        /// </summary>
        /// <param name="linksOpenCount">Словарь ссылок и колличество их открытий</param>
        public async Task SaveOpenLinksCountAsync(IDictionary<string, int> linksOpenCount)
        {//ToDo сохраняем табличным типом
            throw new NotImplementedException();
        }
        /// <summary>
        /// Сохраняет ссылку и ее укороченный вариант в БД.
        /// </summary>
        /// <param name="shortLink"></param>
        /// <param name="longLink"></param>
        public async Task SaveLink(string shortLink, string longLink)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetOriginalLinkAsync(string shortLink)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Возвращает оригинальную ссылку из кеша
        /// </summary>
        /// <param name="shortLink">укороченная ссылка</param>
        public async Task<string> GetOriginalLinkCachedAsync(string shortLink)
        {
            return await CacheClient.GetCachedObjectInternalWithLockAsync(
                shortLink,
                async () => await GetOriginalLinkAsync(shortLink),
                TimeSpan.FromMinutes(CasheConfig.LinksDefaultCachePeriodInMinutes));
        }
    }
}
