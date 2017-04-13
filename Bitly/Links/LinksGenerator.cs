using BitLy.CacheHelper;
using BitLy.DAL.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitLy.DAL.Entities;

namespace LinksFactory
{
    public class LinksGenerator
    {
        //Исключим из массива символов гласные буквы, для исключения вероятности образования неприличных слов
        //Можно поступить коммерческим образом, и раздавать короткие ссылки из 2,3,4 символов.
        private const int ShortUrlLength = 5;
        private const string ShortUrlCharset = "bcdfghjklmnpqrstvwxzBCDFGHJKLMNPQRSTVWXZ0123456789_-~";
        private static readonly string _hostUrl = ConfigurationManager.AppSettings["hostUrl"];

        /// <summary>
        /// Генерация короткой ссылки (используя особый механизм)
        /// </summary>
        /// <param name="nativeLink">Исходная ссылка</param>
        /// <returns>Укороченная ссылка</returns>
        public static async Task<string> GetNewShortLinkAsync(string nativeLink)
        {
            var dbLinks = new DbLinks();
            //1. Проверка сгенерированной ссылки на занятость
            string shortUrl;
            ShortLink existShortUrl;
            do
            {
                shortUrl = GenerateIdentifier(ShortUrlCharset, ShortUrlLength);
                existShortUrl = await dbLinks.GetNativeLinkCachedAsync(shortUrl);
            } while (shortUrl.Equals(existShortUrl?.ShortUrl));


            /*2. Запись в кеш ссылок, свежесгенерированной ссылки, для моментального использования + что бы не получилось что в БД еще пусто.
            т.к. по статистике она сразу же будет использована создателем для проверки работоспособности.*/
          
            CacheClient.SetCachedObject(shortUrl, nativeLink, TimeSpan.FromMinutes(CasheConfig.LinksDefaultCachePeriodInMinutes));

            //TODO 3. Запись в БД отдельной задачей Task.Run или скорее всего через async (еще протестирую производительность)
            ThreadPool.QueueUserWorkItem(async q =>
            {
                try
                {
                    await dbLinks.SaveLink(shortUrl, nativeLink);
                }
                catch (Exception ex)
                {
                    //ToDo подключить логирование
                }
            });
            //Task.Run(async () => await dbLinks.SaveLink(shortLink, nativeLink));

            return $"{_hostUrl}/{shortUrl}";
        }

        private static string GenerateIdentifier(string charset, int length)
        {
            byte[] resultBytes = new byte[length];

            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] random = new byte[1];

            for (int i = 0; i < length; ++i)
            {
                provider.GetBytes(random);
                int pos = (int)(((float)random[0] / byte.MaxValue) * (charset.Length - 1));

                resultBytes[i] = (byte)charset[pos];
            }

            return Encoding.ASCII.GetString(resultBytes);
        }
    }
}
