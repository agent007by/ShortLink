using System;
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
        private static string _connectionString = ConfigurationManager.ConnectionStrings["ShortLink"].ConnectionString;
        /// <summary>
        /// Получение общей статистики переходов по ссылкам
        /// </summary>
        public async Task<IEnumerable<ShortLinkOpenStatistics>> GetShortLinksOpenStatisticsAsync()
        {
            //ToDo
            return await Task.Run(() => new List<ShortLinkOpenStatistics> {
                new ShortLinkOpenStatistics
                {
                    Link = new ShortLink { Id = 1, NativeUrl = "http://mail.ru", ShortUrl = "http://bitly/l/fqwerr" },
                    Statistics = new List<ShortLinkOpenCountStatistic> { new ShortLinkOpenCountStatistic { OpenCount = 100, OpenDate = DateTime.Now } }
                }
                });
        }

        /// <summary>
        /// Сохраняем счетчики переходов по ссылкам
        /// </summary>
        /// <param name="linksRedirectsCount">Словарь ссылок и колличество переходов</param>
        public async Task SaveOpenLinksCountAsync(IDictionary<int, int> linksRedirectsCount)
        {
            //ToDo предусмотреть гарантированное сохранение
            using (var cnn = new SqlConnection(_connectionString))
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
                    cmd.Parameters.AddWithValue("@NewPhoneCalls", dt);

                    try
                    {
                        cnn.Open();
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
            using (var cnn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("[dbo].[ShortLinks_Save]", cnn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1000 })
                {
                    cmd.Parameters.AddWithValue("@NativeLink", nativeLink);
                    cmd.Parameters.AddWithValue("@ShortLink", shortLink);
                    try
                    {
                        cnn.Open();
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

        /// <summary>
        ///Получение информации о ссылке по ShortLink или обо всех ссылках
        /// </summary>
        /// <param name="shortLink"></param>
        /// <returns></returns>
        private async Task<IEnumerable<ShortLink>> GetNativeLinks(string shortLink)
        {
            List<ShortLink> result = new List<ShortLink>();
            using (var cnn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("[dbo].[ShortLinks_Get]", cnn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 60 })
                {
                    if (!string.IsNullOrEmpty(shortLink))
                    {
                        cmd.Parameters.AddWithValue("@ShortLink", shortLink);
                    }

                    try
                    {
                        cnn.Open();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            var item = new ShortLink();
                            while (await reader.ReadAsync())
                            {
                                item.Id = (Int32)reader["ShortLinkId"];
                                item.NativeUrl = reader["NativeUrl"].ToString();
                                item.ShortUrl = reader["ShortUrl"].ToString();
                                item.IsActive = (bool)reader["IsActive"];
                                item.CreateDate = (DateTime)reader["CreateDate"];
                            }
                            result.Add(item);
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
    }
}
