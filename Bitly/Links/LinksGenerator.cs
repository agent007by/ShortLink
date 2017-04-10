using BitLy.CacheHelper;
using BitLy.DAL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinksFactory
{
    public class LinksGenerator
    {
        //Исключим из массива символов гласные буквы, для исключения вероятности образования неприличных слов
        //Используя 5 символов из массива, для составления уникальной ссылки, получим 312,5 Млн вариантов
        //Можно поступить коммерческим образом, и раздавать короткие ссылки из 2,3,4 символов.
        private static char[] encryptionArray
            = new char[]{'b','c','d','f','g','h','j','k','l','m',
                         'n','p','q','r','s','t','v','w','x','z',
                         'B','C','D','F','G','H','J','K','L','M',
                         'N','P','Q','R','S','T','V','W','X','Z',
                         '1','2','3','4','5','6','7','8','9','0'};

        private const int encryptionSymbolsCount = 5;
        /// <summary>
        /// Генерация короткой ссылки (используя особый механизм)
        /// </summary>
        /// <param name="longLink">Исходная ссылка</param>
        /// <param name="hostUrl">Адрес хоста приложения</param>
        /// <returns>Укороченная ссылка</returns>
        public static async Task<string> GetNewShortLinkAsync(string longLink, string hostUrl)
        {
            var shortLink = $"{hostUrl}/{GetRandomSymbols()}";
            //1) Запись в кеш ссылок, свежесгенерированной ссылки, для моментального использования + что бы не получилось что в БД еще пусто.
            CacheClient.SetCachedObject(shortLink, longLink, TimeSpan.FromMinutes(CasheConfig.LinksDefaultCachePeriodInMinutes));
            //TODO 2) Запись в БД отдельной задачей Task.Run или скорее всего через async (еще протестирую производительность)
           // await DbLinks.SaveLink(shortLink, longLink);
            return shortLink;
        }

        /// <summary>
        /// Генерация случайной последовательности символов, для короткой ссылки
        /// </summary>
        /// <returns></returns>
        private static string GetRandomSymbols()
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < encryptionSymbolsCount; i++)
            {
                var randomSymbolPosition = random.Next(0, encryptionArray.Length - 1);
                sb.Append(encryptionArray[randomSymbolPosition]);
            }
            return sb.ToString();
        }
    }
}
