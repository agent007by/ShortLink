using System.Web.Mvc;

namespace Bitly.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Укоротить ссылку";

            return View();
        }
        public ActionResult ShortLinksStatistics()
        {
            ViewBag.Title = "Ваши ссылки";

            return View();
        }
    }
}
