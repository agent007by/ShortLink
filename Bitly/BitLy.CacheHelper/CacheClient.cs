using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace BitLy.CacheHelper
{
    public class CacheClient
    {
        public delegate T ReturnAction<out T>();

        static MemoryCache _cm = MemoryCache.Default;
        static readonly ConcurrentDictionary<string, object> _lockDictionary = new ConcurrentDictionary<string, object>();
        static readonly ConcurrentDictionary<string, Lazy<SemaphoreSlim>> _semaphoresDictionary = new ConcurrentDictionary<string, Lazy<SemaphoreSlim>>();

        #region Caching
        public static T GetCachedObjectInternal<T>(string cacheId, ReturnAction<T> action, TimeSpan timeSpan)
        {
            T obj = GetCachedObject<T>(cacheId);
            if (obj == null)
            {
                obj = action();
                SetCachedObject(cacheId, obj, timeSpan);
            }
            return obj;
        }
        /// <summary>
        /// Получение данных из кеша, с гарантированным одним обращением к методу, указанному в action
        /// </summary>
        /// <typeparam name="T">тип данных, возвращаемого значения</typeparam>
        /// <param name="cacheId">идентификатор кеша</param>
        /// <param name="action">метод вовращающий данные при отсутствии в кеше</param>
        /// <param name="timeSpan">срок жизни кеша</param>
        /// <returns>
        /// объект возврящаемый методом указаннным в action
        /// </returns>        
        public static T GetCachedObjectInternalWithLock<T>(string cacheId, Func<T> action, TimeSpan timeSpan, int wait = 1000)
        {
            T obj = GetCachedObject<T>(cacheId);
            if (obj == null)
            {
                _lockDictionary.GetOrAdd(cacheId, new object());
                var lockObject = _lockDictionary[cacheId];

                lock (lockObject)
                {
                    obj = GetCachedObject<T>(cacheId);
                    if (obj == null)
                    {
                        obj = action();
                        SetCachedObject(cacheId, obj, timeSpan);
                    }
                    object old;
                    _lockDictionary.TryRemove(cacheId, out old);
                }
            }
            return obj;
        }

        public static async Task<T> GetCachedObjectInternalWithLockAsync<T>(string cacheId, Func<Task<T>> action, TimeSpan timeSpan, int wait = 1000)
        {
            T obj = GetCachedObject<T>(cacheId);
            if (obj == null)
            {
                var lazy = _semaphoresDictionary.GetOrAdd(cacheId, new Lazy<SemaphoreSlim>(() => new SemaphoreSlim(1), LazyThreadSafetyMode.ExecutionAndPublication));
                var lockObject = lazy.Value;

                await lockObject.WaitAsync().ConfigureAwait(false);
                try
                {
                    obj = GetCachedObject<T>(cacheId);
                    if (obj == null)
                    {
                        obj = await action().ConfigureAwait(false);
                        SetCachedObject(cacheId, obj, timeSpan);
                    }
                    Lazy<SemaphoreSlim> old;
                    _semaphoresDictionary.TryRemove(cacheId, out old);
                    lockObject.Release();
                }
                catch
                {
                    lockObject.Release();
                    throw;
                }
            }
            return obj;
        }
        public static T GetCachedObject<T>(string cacheId)
        {
            object data = _cm.Get(cacheId);
            return data is T ? (T)data : default(T);
        }
        public static void SetCachedObject(string cacheId, object obj, TimeSpan timeSpan, CacheEntryUpdateCallback refreshAction = null)
        {
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now.Add(timeSpan);
            if (refreshAction != null)
            {
                policy.UpdateCallback = refreshAction;
            }
            if (obj != null)
            {
                _cm.Add(cacheId, obj, policy);
            }
        }
        public static void ClearCache(string cacheId)
        {
            _cm.Remove(cacheId);
        }
        #endregion
    }
}
