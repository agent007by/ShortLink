using BitLy.DAL.EF;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BitLy.DAL.Entities;

namespace LinksFactory
{
    public static class LinkStatisticCounter
    {
        //ToDo вынести в конфигурацию
        const int LockTimeoutMs = 250;
        const int WriteToDbIntervalSec = 10;
        const int WriteToDbMaxCount = 100;
        static DateTime _lastWriteTime = DateTime.Now;
        static ConcurrentDictionary<int, int> _totalOpenLinkCounts = new ConcurrentDictionary<int, int>();
        static readonly object _syncRoot = new object();
        static readonly DbLinks dbLinks = new DbLinks();
        private static readonly string _hostUrl = ConfigurationManager.AppSettings["hostUrl"];

        public static async Task AddRedirectLinkCount(int linkId)
        {
            _totalOpenLinkCounts.AddOrUpdate(linkId, 1, (k, v) => ++v);
            //Проверяем, не подошло ли время очередного ежеминутного сохранения в Бд
            //ToDo или же количество достигнет определенного критического размера.
            if (DateTime.Now.Subtract(_lastWriteTime).TotalSeconds >= WriteToDbIntervalSec || WriteToDbMaxCount <= _totalOpenLinkCounts.Count)
            {
                if (Monitor.TryEnter(_syncRoot, LockTimeoutMs))
                {
                    var totalOpenLinkCounts = _totalOpenLinkCounts;
                    _totalOpenLinkCounts = new ConcurrentDictionary<int, int>();
                    _lastWriteTime = DateTime.Now;
                    Monitor.Exit(_syncRoot);

                    if (totalOpenLinkCounts.Count > 0)
                    {
                        ThreadPool.QueueUserWorkItem(async q =>
                            {
                                try
                                {
                                    var w = new Stopwatch();
                                    w.Start();
                                    //ТО Do в финальном варианте работать через DI , сейчас пока набросок.
                                    await dbLinks.SaveOpenLinksCountAsync(totalOpenLinkCounts);
                                    w.Stop();

                                }
                                catch (Exception ex)
                                {
                                    //ToDo подключить логирование и записать в случае ошибки так же время выполнения w.Elapsed
                                }
                            });
                    }
                }
            }
        }

        public static async Task SaveData()
        {
            await dbLinks.SaveOpenLinksCountAsync(_totalOpenLinkCounts);
        }

        public static async Task<IEnumerable<ShortLinkStatistics>> GetStatistics()
        {
            var statistics = await dbLinks.GetShortLinksOpenStatisticsAsync();
            foreach (var statistic in statistics)
            {
                statistic.Link.ShortUrl = $"{_hostUrl}/{statistic.Link.ShortUrl}";
                statistic.Statistics = statistic.Statistics.OrderByDescending(s => s.CreateDate);
            }
            return statistics;
        }
    }
}
