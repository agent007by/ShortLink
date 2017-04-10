using BitLy.DAL.EF;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LinksFactory
{
    public static class LinkStatisticCounter
    {
        const int LockTimeoutMs = 250;
        const int WriteToDbIntervalSec = 60;
        static DateTime _lastWriteTime = DateTime.Now;
        static ConcurrentDictionary<string, int> _totalOpenLinkCounts = new ConcurrentDictionary<string, int>();
        static readonly object _syncRoot = new object();
        static readonly DbLinks dbLinks = new DbLinks();

        public static async Task AddOpenLinkCount(string link)
        {
            _totalOpenLinkCounts.AddOrUpdate(link, 1, (k, v) => ++v);
            //Проверяем, не подошло ли время очередного ежеминутного сохранения в Бд
            if (DateTime.Now.Subtract(_lastWriteTime).TotalSeconds >= WriteToDbIntervalSec)
            {
                if (Monitor.TryEnter(_syncRoot, LockTimeoutMs))
                {
                    var totalOpenLinkCounts = _totalOpenLinkCounts;
                    _totalOpenLinkCounts = new ConcurrentDictionary<string, int>();
                    _lastWriteTime = DateTime.Now;
                    Monitor.Exit(_syncRoot);

                    if (totalOpenLinkCounts.Count > 0)
                    {
                        ThreadPool.QueueUserWorkItem(q =>
                            {
                                try
                                {
                                    var w = new Stopwatch();
                                    w.Start();
                                    //ТО Do в финальном варианте работать через DI , сейчас пока набросок.
                                     dbLinks.SaveOpenLinksCountAsync(totalOpenLinkCounts);
                                    w.Stop();
                                }
                                catch (Exception ex)
                                {
                                    //ToDo подключить логирование
                                }
                            });
                    }
                }
            }
        }

    }
}
