using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinksFactory;
using System.Collections.Generic;
using System.Linq;

namespace BitLy.BL.LinksFactory.Test
{
    [TestClass]
    public class LinksGeneratorTest
    {
        [TestMethod]
        public async void GetRandomSymbolsTest()
        {
            int linksCount = 10000;
            var uniqueShortLinks = new HashSet<string>();
            for (int i = 0; i < linksCount; i++)
            {
                var shortLinks = await LinksGenerator.GetNewShortLinkAsync("http://testurl.com");
                uniqueShortLinks.Add(shortLinks);
                Assert.IsNotNull(shortLinks);
            }
            var uniqueShortLinkCount = uniqueShortLinks.Distinct().Count();
            Assert.AreEqual(linksCount, uniqueShortLinkCount);
        }
    }
}
