using System.Web.Mvc;


namespace TreeStruct.Controllers
{
    public class HomeController : Controller
    {
        //odpowiada tylko za wyświetlanie strony głównej
        public ActionResult Index()
        {
            return View();
        }
    }
}