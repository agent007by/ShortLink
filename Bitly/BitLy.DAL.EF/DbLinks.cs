using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BitLy.DAL.Repositories;
using BitLy.DAL.Entities;
using BitLy.CacheHelper;
using System.Configuration;
using System.Linq;

namespace BitLy.DAL.EF
{
    //ToDo Log и Timeout в Configuration
    //Прикрутить HelpClass или Dapper для маппинга
    public class DbLinks : ILinkRepository
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ShortLink"].ConnectionString;
        /// <summary>
        /// Получение общей статистики переходов по ссылкам
        /// </summary>
        public async Task<IEnumerable<ShortLinkStatistics>> GetShortLinksOpenStatisticsAsync()
        {
            List<Task> tasks = new List<Task>();

            var getNativeLinksTask = GetNativeLinks();
            tasks.Add(getNativeLinksTask);

            var getShortLinkRedirectCountStatisticsTask = GetShortLinkRedirectCountStatistics();
            tasks.Add(getShortLinkRedirectCountStatisticsTask);

            await Task.WhenAll(tasks).ConfigureAwait(false);
            var links = getNativeLinksTask.Result;
            var statistics = getShortLinkRedirectCountStatisticsTask.Result.ToArray();

            var result = new ConcurrentBag<ShortLinkStatistics>();
            foreach (var link in links.AsParallel())
            {
                result.Add(
                    new ShortLinkStatistics()
                    {
                        Link = link,
                        Statistics = statistics.Where(s => s.ShortLinkId == link.Id)
                    });
            }
            return result;
        }

        /// <summary>
        /// Сохраняем счетчики переходов по ссылкам
        /// </summary>
        /// <param name="linksRedirectsCount">Словарь ссылок и колличество переходов</param>
        public async Task SaveOpenLinksCountAsync(IDictionary<int, int> linksRedirectsCount)
        {
            //ToDo предусмотреть гарантированное сохранение
            using (var cnn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand("[dbo].[ShortLinks_RedirectsAdd]", cnn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1000 })
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ShortLinkId", typeof(int)));
                    dt.Columns.Add(new DataColumn("RedirectCount", typeof(int)));

                    foreach (var item in linksRedirectsCount)
                    {
                        var row = dt.NewRow();
                        row["ShortLinkId"] = item.Key;
                        row["RedirectCount"] = item.Value;
                        dt.Rows.Add(row);
                    }
                    cmd.Parameters.AddWithValue("@Redirects", dt);

                    try
                    {
                        await cnn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Ошибка в при сохранении переходов по ссылкам", ex);
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// Сохраняет ссылку и ее укороченный вариант в БД.
        /// </summary>
        /// <param name="shortLink"></param>
        /// <param name="nativeLink"></param>
        public async Task SaveLink(string shortLink, string nativeLink)
        {
            using (var cnn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand("[dbo].[ShortLinks_Save]", cnn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1000 })
                {
                    //ToDo Сохранять  дату в соответствии с часовым поясом клиента
                    cmd.Parameters.AddWithValue("@NativeLink", nativeLink);
                    cmd.Parameters.AddWithValue("@ShortLink", shortLink);
                    try
                    {
                        await cnn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Ошибка в при сохранении укороченной ссылки", ex);
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }
        }

        public async Task<IEnumerable<ShortLink>> GetNativeLinks()
        {
            return await GetNativeLinks(null);
        }

        public async Task<ShortLink> GetNativeLink(string shortLink)
        {
            var links = await GetNativeLinks(shortLink);
            return links.FirstOrDefault();
        }

        /// <summary>
        /// Возвращает оригинальную ссылку из кеша
        /// </summary>
        /// <param name="shortLink">укороченная ссылка</param>
        public async Task<ShortLink> GetNativeLinkCachedAsync(string shortLink)
        {
            return await CacheClient.GetCachedObjectInternalWithLockAsync(
                shortLink,
                async () => await GetNativeLink(shortLink),
                TimeSpan.FromMinutes(CasheConfig.LinksDefaultCachePeriodInMinutes));
        }

        #region private metods
        /// <summary>
        /// Получение объединенной информации о переходах по ссылкам за все время.
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<ShortLinkRedirectCountStatistics>> GetShortLinkRedirectCountStatistics()
        {
            List<ShortLinkRedirectCountStatistics> result = new List<ShortLinkRedirectCountStatistics>();
            using (var cnn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand("[dbo].[ShortLinks_GetFullStat]", cnn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 60 })
                {
                    try
                    {
                        await cnn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new ShortLinkRedirectCountStatistics
                                {
                                    ShortLinkId = (int)reader["ShortLinkId"],
                                    CreateDate = (DateTime)reader["CreateDate"],
                                    RedirectCount = (int)reader["RedirectCount"]
                                };
                                result.Add(item);
                            }
                        }
                        return result;
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Ошибка в при получение информации о переходов по ссылкам", ex);
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }
        }

        /// <summary>
        ///Получение информации о ссылке по ShortLink или обо всех ссылках
        /// </summary>
        /// <param name="shortLink">null-получить все ссылки</param>
        /// <returns></returns>
        private async Task<IEnumerable<ShortLink>> GetNativeLinks(string shortLink)
        {
            List<ShortLink> result = new List<ShortLink>();
            using (var cnn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand("[dbo].[ShortLinks_Get]", cnn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 60 })
                {
                    cmd.Parameters.AddWithValue("@ShortLink", string.IsNullOrEmpty(shortLink) ? null : shortLink);

                    try
                    {
                        await cnn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new ShortLink();
                                item.Id = (Int32)reader["ShortLinkId"];
                                item.NativeUrl = reader["NativeUrl"].ToString();
                                item.ShortUrl = reader["ShortUrl"].ToString();
                                item.IsActive = (bool)reader["IsActive"];
                                item.CreateDate = (DateTime)reader["CreateDate"];
                                result.Add(item);
                            }
                        }
                        return result;
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Ошибка в при получение информации о ссылке по ShortLink", ex);
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }
        }
        #endregion
    }
}
