﻿using BitLy.CacheHelper;
using BitLy.DAL.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LinksFactory
{
    public class LinksGenerator
    {
        //Исключим из массива символов гласные буквы, для исключения вероятности образования неприличных слов
        //Можно поступить коммерческим образом, и раздавать короткие ссылки из 2,3,4 символов.
        private const int ShortUrlLength = 5;
        private const string ShortUrlCharset = "bcdfghjklmnpqrstvwxzBCDFGHJKLMNPQRSTVWXZ0123456789._-~";
        private static readonly string _hostUrl = ConfigurationManager.AppSettings["hostUrl"];

        /// <summary>
        /// Генерация короткой ссылки (используя особый механизм)
        /// </summary>
        /// <param name="nativeLink">Исходная ссылка</param>
        /// <returns>Укороченная ссылка</returns>
        public static async Task<string> GetNewShortLinkAsync(string nativeLink)
        {
            var shortLink = $"{_hostUrl}/{GenerateIdentifier(ShortUrlCharset, ShortUrlLength)}";
            //ToDo проверка на уникальность сгенерированной ссылки
            //1) Запись в кеш ссылок, свежесгенерированной ссылки, для моментального использования + что бы не получилось что в БД еще пусто.
            CacheClient.SetCachedObject(shortLink, nativeLink, TimeSpan.FromMinutes(CasheConfig.LinksDefaultCachePeriodInMinutes));
            //TODO 2) Запись в БД отдельной задачей Task.Run или скорее всего через async (еще протестирую производительность)
            // await DbLinks.SaveLink(shortLink, longLink);
            return shortLink;
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
